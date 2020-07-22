using FluentValidation.Results;
using GeoTaskManager.Application.Actors.Commands;
using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Actors.Validators;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Actors.Handlers
{
    public class ActorUpdateCommandHandler
        : IRequestHandler<ActorUpdateCommand, UpdateResult>
    {
        private IMediator Mediator { get; }
        private ILogger<ActorUpdateCommandHandler> Logger { get; }

        public ActorUpdateCommandHandler(IMediator mediator,
            ILogger<ActorUpdateCommandHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle(ActorUpdateCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(AppLogEvent.HandleRequest,
                    "Handle Actor Update {Command}", command.ToDictionary());

                if (command is null || string.IsNullOrWhiteSpace(command.Id))
                {
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Actor Update Command is empty");
                    return ErrorResult("Actor Update Command is empty");
                }

                var validator = new ActorUpdateCommandValidator();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    var validationErrors = validationResult.Errors
                                            .Select(x => x.ErrorMessage)
                                            .ToList();
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Actor Update Command validation error. " +
                        "Command={Command}. Error={Error}.",
                        command.ToDictionary(), validationErrors);
                    return ErrorResult(validationErrors);
                }

                var oldActorResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Actor>(command.Id))
                    .ConfigureAwait(false);

                if (!oldActorResponse.Success)
                {
                    Logger.LogWarning(AppLogEvent.HandleErrorResponse,
                        "Get actor for update error. Id={Id}. Error={Error}",
                        command.Id, oldActorResponse.Errors);
                    return ErrorResult(oldActorResponse.Errors);
                }
                var oldActor = oldActorResponse.Entity;

                var currentActorRequest =
                    new DbGetActorByNameRequest
                        (command.CurrentPrincipal?.Identity?.Name);
                var currentActorResponse = await Mediator
                    .Send(currentActorRequest)
                    .ConfigureAwait(false);
                var currentActor = currentActorResponse.Success
                    ? currentActorResponse.Entity
                    : null;

                Actor newActor = command.ToActor(oldActor);

                // Check duplicates for new login
                if (newActor.Login != oldActor.Login)
                {
                    var duplicateRequest = new DbGetActorByNameRequest
                        (newActor.Login);
                    var duplicateResponse = await Mediator
                        .Send(duplicateRequest)
                        .ConfigureAwait(false);
                    if (duplicateResponse.Success
                        && duplicateResponse.Entity != null)
                    {
                        Logger.LogWarning(AppLogEvent.RequestValidationError,
                            "Validation command error. Command={command}. " +
                            "Error={Error}.",
                            command.ToDictionary(), "Duplicated actor login");
                        return ErrorResult("Duplicated actor login");
                    }
                }

                var validatorBeforeSave = new ActorBeforeSaveValidator();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(newActor)
                    .ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Update Actor validation error. Entity={Entity}. " +
                        "Error={Error}.", newActor.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var permissionValidateResult = await CheckPermissionAsync
                    (oldActor, newActor, currentActor)
                    .ConfigureAwait(false);
                if (!permissionValidateResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Current actor has no rights to update Actor. " +
                        "Actor={Actor}. Entity before update={OldEntity}. " +
                        "Entity after update={NewEntity}. Error={Error}.",
                        currentActor.ToDictionary(), oldActor.ToDictionary(),
                        newActor.ToDictionary(),
                        permissionValidateResult.Errors
                            .Select(x => x.ErrorMessage));
                    return ErrorResult(permissionValidateResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var updateCommand = new DbUpdateCommand<Actor>(newActor);
                var updateResult = await Mediator.Send(updateCommand)
                                                  .ConfigureAwait(false);
                return updateResult;
            }

            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Actor update exception");
                return ErrorResult("Actor update exception");
            }
        }

        private async Task<ValidationResult> CheckPermissionAsync
            (Actor oldActor, Actor newActor, Actor currentActor)
        {
            var checkModel = new CheckUpdatePermissionModel<Actor>
            {
                Actor = currentActor,
                EntityBeforeUpdate = oldActor,
                EntityAfterUpdate = newActor
            };
            var validator = new ActorUpdatePermissionValidator();
            var validatorResult = await validator.ValidateAsync(checkModel)
                .ConfigureAwait(false);
            return validatorResult;
        }

        private UpdateResult ErrorResult(IEnumerable<string> errors)
        {
            var result = new UpdateResult
            {
                Success = false
            };
            result.Errors.AddRange(errors);
            return result;
        }

        private UpdateResult ErrorResult(string error)
            => ErrorResult(new string[] { error });
    }
}
