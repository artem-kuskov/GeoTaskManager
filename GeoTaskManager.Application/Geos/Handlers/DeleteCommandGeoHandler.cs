using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Events;
using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.Geos.Validators;
using GeoTaskManager.Application.Projects.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _EntityType = GeoTaskManager.Application.Geos.Models.Geo;


namespace GeoTaskManager.Application.Geos.Handlers
{
    public class DeleteCommandGeoHandler
        : IRequestHandler<DeleteCommand<_EntityType>, DeleteResult>
    {
        private IMediator Mediator { get; }
        private ILogger<DeleteCommandGeoHandler> Logger { get; }

        public DeleteCommandGeoHandler
            (IMediator mediator,
            ILogger<DeleteCommandGeoHandler> logger)
        {
            Logger = logger;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<DeleteResult> Handle
            (DeleteCommand<_EntityType> command,
            CancellationToken cancellationToken)
        {
            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Entity Delete empty command");
                return ErrorResult("Entity Delete empty command");
            }

            try
            {
                var validator = new DeleteCommandValidator<_EntityType>();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Validation command error. Command={Command}. " +
                        "Error={Error}.",
                        command.ToDictionary(), validationResult.Errors);
                    return ErrorResult
                        (validationResult.Errors.Select(x => x.ErrorMessage));
                }

                var entityResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<_EntityType>(command.Id))
                    .ConfigureAwait(false);
                if (entityResponse.Success != true)
                {
                    Logger.LogWarning(AppLogEvent.HandleNullResponse,
                        "Handle Entity Delete Command " +
                        "can not get the entity. " +
                        "Command={Command}. Error={Error}.",
                        command.ToDictionary(), entityResponse.Errors);
                    return ErrorResult(entityResponse.Errors);
                }
                var entity = entityResponse.Entity;

                // Get Actor for current user by user name
                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (command.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                Actor currentActor = null;
                if (currentActorResponse.Success)
                {
                    currentActor = currentActorResponse.Entity;
                }

                // Get project role of current actor
                var projectResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Project>(entity.ProjectId))
                    .ConfigureAwait(false);
                ActorRole currentProjectRole = null;
                if (projectResponse.Success)
                {
                    projectResponse.Entity.ProjectActorRoles
                        .TryGetValue(currentActor.Id, out currentProjectRole);
                }

                var checkPermissionResult = await CheckPermission(entity,
                    currentActor, command.HardMode, currentProjectRole)
                    .ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Entity Delete permission not passed. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}. " +
                        "CurrentActorProjectRole={CurrentActorProjectRole}" +
                        "Error={Error}.",
                        entity.ToDictionary(), currentActor?.ToDictionary(),
                        currentProjectRole.Name, checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                if (command.HardMode)
                {
                    await Mediator.Publish
                        (new BeforeEntityDelete<_EntityType>(entity))
                        .ConfigureAwait(false);

                    var result = await HardDelete(command, entity)
                        .ConfigureAwait(false);

                    await Mediator.Publish
                        (new AfterEntityDelete<_EntityType>(entity))
                        .ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return await SoftDelete(entity)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResult("Not found");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        private async Task<DeleteResult> HardDelete
            (DeleteCommand<_EntityType> command, _EntityType entity)
        {
            return await Mediator
                .Send(new DbDeleteCommand<_EntityType>(entity.Id))
                .ConfigureAwait(false);
        }

        private async Task<DeleteResult> SoftDelete(_EntityType entity)
        {
            entity.IsArchived = true;
            var updateResult = await Mediator
                .Send(new DbUpdateCommand<_EntityType>(entity))
                .ConfigureAwait(false);
            return updateResult.ToDeleteResult();
        }

        private async Task<DeleteResult> CheckPermission(_EntityType entity,
            Actor currentActor, bool hardMode, ActorRole currentProjectRole)
        {
            var checkModel = new CheckDeletePermissionModel<_EntityType>
            {
                Entity = entity,
                Actor = currentActor,
                HardMode = hardMode,
                ProjectActorRole = currentProjectRole
            };
            var validator = new DeletePermissionValidator<_EntityType>();
            var validatorResult = await validator
                .ValidateAsync(checkModel)
                .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult
                    (validatorResult.Errors.Select(x => x.ErrorMessage));
            }
            return new DeleteResult { Success = true };
        }

        private DeleteResult ErrorResult(IEnumerable<string> errors)
        {
            var result = new DeleteResult { Success = false };
            result.Errors.AddRange(errors);
            return result;
        }

        private DeleteResult ErrorResult(string error)
            => ErrorResult(new string[] { error });
    }
}
