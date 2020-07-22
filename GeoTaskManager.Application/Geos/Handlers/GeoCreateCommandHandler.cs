using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
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
using _TCreateCommand = GeoTaskManager.Application.Geos.Commands.GeoCreateCommand;
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;


namespace GeoTaskManager.Application.Geos.Handlers
{
    public class GeoCreateCommandHandler
        : IRequestHandler<_TCreateCommand, CreateResult>
    {
        private IMediator Mediator { get; }
        private ILogger<GeoCreateCommandHandler> Logger { get; }

        public GeoCreateCommandHandler(IMediator mediator,
            ILogger<GeoCreateCommandHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle(_TCreateCommand command,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Geo Create Command. Command={Command}",
                command.ToDictionary());

            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Geo Create Command got empty command");
                return ErrorResult("Empty Geo Create Command");
            }

            try
            {
                var validator = new CreateCommandValidator<_TCreateCommand>();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Validation command error. Command={Command}. " +
                        "Error={Error}.",
                        command.ToDictionary(), validationResult.Errors);
                    return ErrorResult(validationResult.Errors
                                        .Select(x => x.ErrorMessage));
                }

                // Get Actor for current user by user name
                Actor createdBy = null;
                var currentUserName = command.CurrentPrincipal?
                                            .Identity?
                                            .Name;
                var creatorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest(currentUserName))
                    .ConfigureAwait(false);
                if (creatorResponse.Success)
                {
                    createdBy = creatorResponse.Entity;
                }

                var entity = new _TEntity()
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    Description = command.Description,
                    IsArchived = command.IsArchived,
                    Title = command.Title,
                    GeoJson = command.GeoJson,
                    ProjectId = command.ProjectId
                };

                var validatorBeforeSave = new BeforeSaveValidator<_TEntity>();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(entity).ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestNotValid,
                        "Geo validation error. Entity={Entity}. " +
                        "Error={Error}.",
                        entity.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                                        .Select(x => x.ErrorMessage));
                }

                // Get project role of current actor
                var projectResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Project>(entity.ProjectId))
                    .ConfigureAwait(false);
                ActorRole currentProjectRole = null;
                if (projectResponse.Success)
                {
                    projectResponse.Entity.ProjectActorRoles
                        .TryGetValue(createdBy.Id, out currentProjectRole);
                }

                var checkPermissionResult = await CheckPermission(entity,
                    createdBy, currentProjectRole)
                    .ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Geo Create permission error. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}." +
                        "CurrentActorProjectRole={CurrentActorProjectRole}." +
                        " Error={Error}.",
                        entity.ToDictionary(), createdBy?.ToDictionary(),
                        currentProjectRole.Name, checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                return await Mediator
                    .Send(new DbCreateCommand<_TEntity>(entity))
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResult("Not found");
            }
        }

        private async Task<CreateResult> CheckPermission
            (_TEntity entity, Actor currentActor, ActorRole currentProjectRole)
        {
            var checkModel = new CheckCreatePermissionModel<_TEntity>
            {
                Entity = entity,
                Actor = currentActor,
                ProjectActorRole = currentProjectRole
            };
            var validator = new CreatePermissionValidator<_TEntity>();
            var validatorResult = await validator.ValidateAsync(checkModel)
                                                 .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult(validatorResult.Errors
                                        .Select(x => x.ErrorMessage));
            }
            return new CreateResult { Success = true };
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
