using FluentValidation.Results;
using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;
using _TUpdateCommand = GeoTaskManager.Application.Projects.Commands.ProjectUpdateCommand;

namespace GeoTaskManager.Application.Projects.Handlers
{
    public class ProjectUpdateCommandHandler
        : IRequestHandler<_TUpdateCommand, UpdateResult>
    {
        private IMediator Mediator { get; }
        private ILogger<ProjectUpdateCommandHandler> Logger { get; }

        public ProjectUpdateCommandHandler
            (IMediator mediator,
            ILogger<ProjectUpdateCommandHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle(_TUpdateCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(AppLogEvent.HandleRequest,
                    "Handle Project Update Command {Command}",
                    command.ToDictionary());

                if (command is null || string.IsNullOrWhiteSpace(command.Id))
                {
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Project Update Command is empty");
                    return ErrorResult("Project Update Command is empty");
                }

                var validator = new ProjectUpdateCommandValidator();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    var validationErrors = validationResult.Errors
                                            .Select(x => x.ErrorMessage)
                                            .ToList();
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Project Update Command validation error. " +
                        "Command={Command}. Error={Error}.",
                        command.ToDictionary(), validationErrors);
                    return ErrorResult(validationErrors);
                }

                var oldEntityResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<_TEntity>(command.Id))
                    .ConfigureAwait(false);

                if (!oldEntityResponse.Success)
                {
                    Logger.LogWarning(AppLogEvent.HandleErrorResponse,
                        "Get entity for update error. Id={Id}. Error={Error}",
                        command.Id, oldEntityResponse.Errors);
                    return ErrorResult(oldEntityResponse.Errors);
                }
                var oldEntity = oldEntityResponse.Entity;

                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (command.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                var currentActor = currentActorResponse.Success
                    ? currentActorResponse.Entity
                    : null;

                _TEntity newEntity = await BuildUpdatedEntity(command,
                    oldEntity)
                    .ConfigureAwait(false);

                var validatorBeforeSave = new BeforeSaveValidator<_TEntity>();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(newEntity)
                    .ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Update Project validation error. Entity={Entity}. " +
                        "Error={Error}.", newEntity.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var permissionValidateResult = await CheckPermissionAsync
                    (oldEntity, newEntity, currentActor)
                    .ConfigureAwait(false);
                if (!permissionValidateResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Current actor has no rights to update entity. " +
                        "Actor={Actor}. Entity before update={OldEntity}. " +
                        "Entity after update={NewEntity}. Error={Error}.",
                        currentActor.ToDictionary(), oldEntity.ToDictionary(),
                        newEntity.ToDictionary(),
                        permissionValidateResult.Errors
                            .Select(x => x.ErrorMessage));
                    return ErrorResult(permissionValidateResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var updateResult = await Mediator
                    .Send(new DbUpdateCommand<_TEntity>(newEntity))
                    .ConfigureAwait(false);
                return updateResult;
            }

            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Project update exception");
                return ErrorResult("Project update exception");
            }
        }

        private async Task<_TEntity> BuildUpdatedEntity
            (_TUpdateCommand command, _TEntity oldEntity)
        {
            var newEntity = new _TEntity
            {
                CreatedAt = oldEntity.CreatedAt,
                CreatedBy = oldEntity.CreatedBy,
                Description = command.Description,
                Id = oldEntity.Id,
                IsArchived = command.IsArchived,
                Title = command.Title,
                IsMap = command.IsMap,
                MapProvider = command.MapProvider,
                ShowMap = command.ShowMap
            };
            newEntity.Layers.AddRange(command.Layers);
            command.MapParameters
                .ToList()
                .ForEach(x => newEntity.MapParameters.TryAdd(x.Key, x.Value));

            // There is no necessary to check actor Ids when they are the same.
            if (command.ProjectActorRoles
                .All(x => oldEntity.ProjectActorRoles.ContainsKey(x.Key)))
            {
                command.ProjectActorRoles
                    .ToList()
                    .ForEach(x => newEntity.ProjectActorRoles
                        .TryAdd(x.Key, x.Value));
            }
            else
            {
                var checkingActorIds = command.ProjectActorRoles.Keys;
                var repResponse = await Mediator
                    .Send(new DbListRequest<Actor>(checkingActorIds))
                    .ConfigureAwait(false);
                if (repResponse.Success)
                {
                    var actorIdSet = new HashSet<string>
                        (repResponse.Entities.Select(x => x.Id));
                    command.ProjectActorRoles
                    .ToList()
                    .ForEach(x =>
                    {
                        if (actorIdSet.Contains(x.Key))
                        {
                            newEntity.ProjectActorRoles.TryAdd(x.Key, x.Value);
                        }
                    });
                }
            }
            return await Task.FromResult(newEntity).ConfigureAwait(false);
        }

        private async Task<ValidationResult> CheckPermissionAsync
            (_TEntity oldEntity, _TEntity newEntity, Actor currentActor)
        {
            var checkModel = new CheckUpdatePermissionModel<_TEntity>
            {
                Actor = currentActor,
                EntityBeforeUpdate = oldEntity,
                EntityAfterUpdate = newEntity
            };
            var validator = new UpdatePermissionValidator<_TEntity>();
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
