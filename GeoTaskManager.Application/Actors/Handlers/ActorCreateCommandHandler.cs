using GeoTaskManager.Application.Actors.Commands;
using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Actors.Validators;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Actors.Handlers
{
    public class ActorCreateCommandHandler
        : IRequestHandler<ActorCreateCommand, CreateResult>
    {
        private ILogger<ActorCreateCommandHandler> Logger { get; }
        private IMediator Mediator { get; }

        public ActorCreateCommandHandler(IMediator mediator,
            ILogger<ActorCreateCommandHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle(ActorCreateCommand command,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Actor Create Command. Command={Command}",
                command.ToDictionary());

            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Create Actor Command got empty command");
                return ErrorResult("Empty Actor Create command");
            }

            try
            {
                var validator = new ActorCreateCommandValidator();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Validation command error. Command = {command}. " +
                        "Error = {Error}",
                        command.ToDictionary(), validationResult.Errors);
                    return ErrorResult(validationResult.Errors
                                        .Select(x => x.ErrorMessage));
                }

                // Check duplicates
                var dbActorRequest = new DbGetActorByNameRequest(command.Login);
                var duplicateResponse = await Mediator.Send(dbActorRequest)
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

                // Get Actor for current user by user name
                Actor createdBy = null;
                if (!command.DataSeedMode)
                {
                    var currentUserName = command.CurrentPrincipal?
                                                 .Identity?
                                                 .Name;
                    var creatorRequest = new DbGetActorByNameRequest
                        (currentUserName);
                    var creatorResponse = await Mediator.Send(creatorRequest)
                                                      .ConfigureAwait(false);
                    if (creatorResponse.Success)
                    {
                        createdBy = creatorResponse.Entity;
                    }
                }

                var actor = command.ToActor(createdBy?.Id);

                var validatorBeforeSave = new ActorBeforeSaveValidator();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(actor)
                    .ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestNotValid,
                        "Actor validation error. Entity={Entity}. " +
                        "Error={Error}",
                        actor.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                                        .Select(x => x.ErrorMessage));
                }

                if (!command.DataSeedMode)
                {
                    var checkPermissionResult =
                        await CheckPermission(actor, createdBy)
                                             .ConfigureAwait(false);
                    if (!checkPermissionResult.Success)
                    {
                        Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                            "Actor check create permission error. " +
                            "Entity={Entity}. CurrentActor={CurrentActor}." +
                            " Error={Error}",
                            actor.ToDictionary(), createdBy?.ToDictionary(),
                            checkPermissionResult.Errors);
                        return checkPermissionResult;
                    }
                }

                var createCommand = new DbCreateCommand<Actor>(actor);
                var createResult = await Mediator.Send(createCommand)
                                                 .ConfigureAwait(false);
                return createResult;
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResult("Not found");
            }
        }

        private async Task<CreateResult> CheckPermission(Actor actor,
            Actor currentActor)
        {
            var checkModel = new CheckCreatePermissionModel<Actor>
            {
                Entity = actor,
                Actor = currentActor
            };
            var validator = new ActorCreatePermissionValidator();
            var validatorResult = await validator.ValidateAsync(checkModel)
                                                 .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult(validatorResult.Errors
                                        .Select(x => x.ErrorMessage));
            }
            return new CreateResult { Success = true, Id = actor.Id };
        }

        private CreateResult ErrorResult(IEnumerable<string> errors)
        {
            var result = new CreateResult { Success = false };
            result.Errors.AddRange(errors);
            return result;
        }

        private CreateResult ErrorResult(string error)
        {
            return ErrorResult(new string[] { error });
        }
    }
}
