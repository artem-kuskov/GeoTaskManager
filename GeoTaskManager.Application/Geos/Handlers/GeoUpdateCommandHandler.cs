using FluentValidation.Results;
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
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;
using _TUpdateCommand = GeoTaskManager.Application.Geos.Commands.GeoUpdateCommand;

namespace GeoTaskManager.Application.Geos.Handlers
{
    public class GeoUpdateCommandHandler
        : IRequestHandler<_TUpdateCommand, UpdateResult>
    {
        private IMediator Mediator { get; }
        private ILogger<GeoUpdateCommandHandler> Logger { get; }

        public GeoUpdateCommandHandler
            (IMediator mediator, ILogger<GeoUpdateCommandHandler> logger)
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
                    "Handle Geo Update Command {Command}",
                    command.ToDictionary());

                if (command is null || string.IsNullOrWhiteSpace(command.Id))
                {
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Geo Update Command is empty");
                    return ErrorResult("Geo Update Command is empty");
                }

                // Validate Update Command
                var validator = new GeoUpdateCommandValidator();
                var validationResult = await validator
                    .ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    var validationErrors = validationResult.Errors
                                            .Select(x => x.ErrorMessage)
                                            .ToList();
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Geo Update Command validation error. " +
                        "Command={Command}. Error={Error}.",
                        command.ToDictionary(), validationErrors);
                    return ErrorResult(validationErrors);
                }

                // Get entity before update
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

                // Get current actor
                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (command.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                var currentActor = currentActorResponse.Success
                    ? currentActorResponse.Entity
                    : null;

                // Build updated entity
                _TEntity newEntity = await BuildUpdatedEntity
                        (command, oldEntity)
                    .ConfigureAwait(false);

                // Validate updated entity before update
                var validatorBeforeSave = new BeforeSaveValidator<_TEntity>();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(newEntity)
                    .ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Update Geo validation error. Entity={Entity}. " +
                        "Error={Error}.", newEntity.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                // Get project role of current actor
                var projectResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Project>
                        (oldEntity.ProjectId))
                    .ConfigureAwait(false);
                ActorRole currentProjectRole = null;
                if (projectResponse.Success)
                {
                    projectResponse.Entity.ProjectActorRoles
                        .TryGetValue(currentActor.Id, out currentProjectRole);
                }

                // Check permission to update
                var permissionValidateResult = await CheckPermissionAsync
                    (oldEntity, newEntity, currentActor, currentProjectRole)
                    .ConfigureAwait(false);
                if (!permissionValidateResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Current actor has no rights to update entity. " +
                        "Actor={Actor}. " +
                        "CurrentActorProjectRole={CurrentActorProjectRole}. " +
                        "Entity before update={OldEntity}. " +
                        "Entity after update={NewEntity}. Error={Error}.",
                        currentActor.ToDictionary(), currentProjectRole,
                        oldEntity.ToDictionary(), newEntity.ToDictionary(),
                        permissionValidateResult.Errors
                            .Select(x => x.ErrorMessage));
                    return ErrorResult(permissionValidateResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                // Save updated entity
                var updateResult = await Mediator
                    .Send(new DbUpdateCommand<_TEntity>(newEntity))
                    .ConfigureAwait(false);
                return updateResult;
            }

            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Geo update exception");
                return ErrorResult("Geo update exception");
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
                GeoJson = command.GeoJson,
                ProjectId = oldEntity.ProjectId
            };

            return await Task.FromResult(newEntity).ConfigureAwait(false);
        }

        private async Task<ValidationResult> CheckPermissionAsync
            (_TEntity oldEntity, _TEntity newEntity, Actor currentActor,
            ActorRole currentProjectRole)
        {
            var checkModel = new CheckUpdatePermissionModel<_TEntity>
            {
                Actor = currentActor,
                EntityBeforeUpdate = oldEntity,
                EntityAfterUpdate = newEntity,
                NewProjectRole = currentProjectRole,
                OldProjectRole = currentProjectRole
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
