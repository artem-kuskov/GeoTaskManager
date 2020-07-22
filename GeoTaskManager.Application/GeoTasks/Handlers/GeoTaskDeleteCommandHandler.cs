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
using GeoTaskManager.Application.GeoTasks.Mappers;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Validators;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _TJsonPatchOperation = Microsoft.AspNetCore.JsonPatch.Operations.OperationType;

namespace GeoTaskManager.Application.GeoTasks.Handlers
{
    public class GeoTaskDeleteCommandHandler
        : IRequestHandler<DeleteCommand<GeoTask>, DeleteResult>
    {
        private ILogger<GeoTaskDeleteCommandHandler> Logger { get; }
        private IMediator Mediator { get; }

        public GeoTaskDeleteCommandHandler
            (ILogger<GeoTaskDeleteCommandHandler> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<DeleteResult> Handle(DeleteCommand<GeoTask> command,
            CancellationToken cancellationToken)
        {
            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle delete task with empty command");
                return ErrorResult("Empty Geo Task Delete command");
            }

            try
            {
                var validator = new GeoTaskDeleteCommandValidator();
                var validationResult = await validator
                    .ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Validation command error. Command={Command}. " +
                        "Error={Error}.", command.ToDictionary(),
                        validationResult.Errors);
                    return ErrorResult(validationResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var entityResponse = await GetGeoTaskAsync(command.Id)
                    .ConfigureAwait(false);
                if (entityResponse.Success != true)
                {
                    Logger.LogWarning(AppLogEvent.HandleNullResponse,
                        "Handle delete Geo Task can not get the task. " +
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

                Project project = await GetProject(entity.ProjectId)
                    .ConfigureAwait(false);

                var checkPermissionResult = await CheckPermission(entity,
                    currentActor, project, command.HardMode)
                    .ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "GeoTask check delete permission not passed. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}. " +
                        "Project={Project}. Error={Error}.",
                        entity.ToDictionary(), currentActor?.ToDictionary(),
                        project?.ToDictionary(), checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                if (command.HardMode)
                {
                    await Mediator.Publish
                        (new BeforeEntityDelete<GeoTask>(entity))
                        .ConfigureAwait(false);

                    var result = await Mediator
                        .Send(new DbDeleteCommand<GeoTask>(entity.Id))
                        .ConfigureAwait(false);

                    await Mediator.Publish
                        (new AfterEntityDelete<GeoTask>(entity))
                        .ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return await SoftDelete(command, entity, currentActor)
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

        private async Task<EntityResponse<GeoTask>> GetGeoTaskAsync
            (string taskId)
            => await Mediator
                .Send(new DbGetEntityByIdRequest<GeoTask>(taskId))
                .ConfigureAwait(false);

        private async Task<DeleteResult> SoftDelete
            (DeleteCommand<GeoTask> command, GeoTask entity, Actor currentActor)
        {
            var historyElement = new GeoTaskHistory
            {
                ChangedAt = DateTime.UtcNow,
                Description = command.MessageDescription,
                Title = command.MessageTitle,
                ChangedBy = currentActor,
            };
            historyElement.Operations.Add(new Operation
            {
                OperationType = _TJsonPatchOperation.Replace,
                OldValue = entity.IsArchived,
                NewValue = true,
                Path = $"/{nameof(entity.IsArchived)}"
            });
            entity.History.Add(historyElement);

            entity.IsArchived = true;

            var updateResult = await Mediator
                .Send(new DbUpdateCommand<GeoTask>(entity))
                .ConfigureAwait(false);
            return updateResult.ToDeleteResult();
        }

        private async Task<Project> GetProject(string projectId)
        {
            if (String.IsNullOrWhiteSpace(projectId))
            {
                return null;
            }
            var projectRequest = await Mediator
                .Send(new DbGetEntityByIdRequest<Project>(projectId))
                .ConfigureAwait(false);
            return !projectRequest.Success ? null : projectRequest.Entity;
        }

        private async Task<DeleteResult> CheckPermission(GeoTask entity,
            Actor currentActor, Project project, bool hardMode)
        {
            ActorRole currentActorProjectRole = null;
            if (project != null && currentActor != null)
            {
                project.ProjectActorRoles.TryGetValue(currentActor.Id,
                    out currentActorProjectRole);
            }
            var checkModel = new CheckDeletePermissionModel<GeoTask>
            {
                Entity = entity,
                Actor = currentActor,
                ProjectActorRole = currentActorProjectRole,
                HardMode = hardMode
            };
            var validator = new GeoTaskDeletePermissionValidator();
            var validatorResult = await validator.ValidateAsync(checkModel)
                .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult(validatorResult.Errors
                    .Select(x => x.ErrorMessage));
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
        {
            return ErrorResult(new string[] { error });
        }
    }
}
