using FluentValidation.Results;
using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.Application.GeoTasks.Commands;
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

namespace GeoTaskManager.Application.GeoTasks.Handlers
{
    public class GeoTaskUpdateCommandHandler
        : IRequestHandler<GeoTaskUpdateCommand, UpdateResult>
    {
        private IMediator Mediator { get; }
        private ILogger<GeoTaskUpdateCommandHandler> Logger { get; }
        private Project OldProject { get; set; }
        private Project NewProject { get; set; }

        public GeoTaskUpdateCommandHandler
            (IGeoTaskManagerDbContext dbContext,
            ILogger<GeoTaskUpdateCommandHandler> logger,
            IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle(GeoTaskUpdateCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(AppLogEvent.HandleRequest,
                    "Handle update task {Command}", command.ToDictionary());

                if (command is null || string.IsNullOrWhiteSpace(command.Id))
                {
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Handle update task command with empty request");
                    return ErrorResult("Command empty argument");
                }

                var validator = new GeoTaskUpdateCommandValidator();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    var validationErrors = validationResult.Errors
                        .Select(x => x.ErrorMessage).ToList();
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Task update command validation error. " +
                        "Command={Command}. Error={Error}",
                        command.ToDictionary(), validationErrors);
                    return ErrorResult(validationErrors);
                }

                var oldGeoTaskResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<GeoTask>(command.Id))
                    .ConfigureAwait(false);
                if (!oldGeoTaskResponse.Success)
                {
                    Logger.LogWarning(AppLogEvent.HandleErrorResponse,
                        "Get task for update error. Id={Id}. Error={Error}.",
                        command.Id, oldGeoTaskResponse.Errors);
                    return ErrorResult(oldGeoTaskResponse.Errors);
                }
                var oldGeoTask = oldGeoTaskResponse.Entity;

                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (command.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                var currentActor = currentActorResponse.Entity;

                OldProject = await GetProjectAsync(oldGeoTask.ProjectId)
                    .ConfigureAwait(false);

                // Check Project Id
                NewProject = null;
                if (command.ProjectId != OldProject?.Id)
                {
                    NewProject = await GetProjectAsync(command.ProjectId)
                        .ConfigureAwait(false);
                }
                else
                {
                    NewProject = OldProject;
                }

                GeoTask newGeoTask = await BuildUpdatedGeoTask(command,
                    oldGeoTask, currentActor).ConfigureAwait(false);

                var validatorBeforeSave = new GeoTaskBeforeSaveValidator();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(newGeoTask)
                    .ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Update GeoTask validation error. " +
                        "Entity={Entity}. Error={Error}.",
                        newGeoTask.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                ActorRole oldProjectRole = null;
                OldProject?.ProjectActorRoles?
                    .TryGetValue(currentActor.Id, out oldProjectRole);
                ActorRole newProjectRole = null;
                if (oldGeoTask.ProjectId != newGeoTask.ProjectId)
                {
                    NewProject?.ProjectActorRoles?
                        .TryGetValue(currentActor.Id, out newProjectRole);
                }
                else
                {
                    newProjectRole = oldProjectRole;
                }

                var permissionValidateResult = await CheckPermissionAsync
                        (oldGeoTask, newGeoTask, currentActor,
                        oldProjectRole, newProjectRole)
                    .ConfigureAwait(false);
                if (!permissionValidateResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Current actor has no rights to update GeoTask. " +
                        "Actor={Actor}. Entity before update={OldEntity}." +
                        " Entity after update={NewEntity}. Error={Error}.",
                        currentActor.ToDictionary(), oldGeoTask.ToDictionary(),
                        newGeoTask.ToDictionary(),
                        permissionValidateResult.Errors
                            .Select(x => x.ErrorMessage));
                    return ErrorResult(permissionValidateResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var updateResult = await Mediator
                    .Send(new DbUpdateCommand<GeoTask>(newGeoTask))
                    .ConfigureAwait(false);
                return updateResult;
            }

            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Geo task update exception");
                return ErrorResult("Geo task update exception");
            }
        }

        private async Task<Project> GetProjectAsync(string id)
            => (await Mediator.Send(new DbGetEntityByIdRequest<Project>(id))
                              .ConfigureAwait(false)
                ).Entity;

        private async Task<GeoTask> BuildUpdatedGeoTask
            (GeoTaskUpdateCommand command, GeoTask oldGeoTask,
            Actor currentActor)
        {
            var newGeoTask = new GeoTask
            {
                CreatedAt = oldGeoTask.CreatedAt,
                CreatedBy = oldGeoTask.CreatedBy,
                Description = command.Description,
                Id = oldGeoTask.Id,
                IsArchived = command.IsArchived,
                PlanFinishAt = command.PlanFinishAt,
                PlanStartAt = command.PlanStartAt,
                Title = command.Title,
                Status = command.Status,
            };

            var allActors = await GetActorsAsync(
                    command.AssistentActorsIds
                        .Concat(command.ObserverActorsIds)
                        .Append(command.ResponsibleActorId)
                    ).ConfigureAwait(false);
            newGeoTask.AssistentActors.AddRange(
                command.AssistentActorsIds
                    .Select(x => allActors.FirstOrDefault(a => a.Id == x))
                    .Where(a => a != null));
            newGeoTask.ObserverActors.AddRange(
                command.ObserverActorsIds
                    .Select(x => allActors.FirstOrDefault(a => a.Id == x))
                    .Where(a => a != null));
            newGeoTask.ResponsibleActor = allActors
                .FirstOrDefault(a => a.Id == command.ResponsibleActorId);

            // Check that Geo Ids exist in repository
            newGeoTask.GeosIds.AddRange(await GetGeosIdsAsync(command.GeosIds)
                .ConfigureAwait(false));

            newGeoTask.History.AddRange(oldGeoTask.History);

            newGeoTask.ProjectId = NewProject?.Id;

            if (oldGeoTask.Status != command.Status)
            {
                newGeoTask.StatusChangedAt = DateTime.UtcNow;
            }
            else
            {
                newGeoTask.StatusChangedAt = oldGeoTask.StatusChangedAt;
            }

            newGeoTask.History.Add(BuildNewHistoryElement
                (newGeoTask, oldGeoTask, command, currentActor));
            return newGeoTask;
        }

        private GeoTaskHistory BuildNewHistoryElement(GeoTask newGeoTask,
            GeoTask oldGeoTask, GeoTaskUpdateCommand command,
            Actor currentActor)
        {
            var historyRecord = new GeoTaskHistory()
            {
                ChangedAt = DateTime.UtcNow,
                ChangedBy = currentActor,
                Description = command.MessageDescription,
                Title = command.MessageTitle
            };
            historyRecord.Operations
                .AddRange(newGeoTask.ToHistoryOperations(oldGeoTask));
            return historyRecord;
        }

        private async Task<IEnumerable<Actor>> GetActorsAsync
            (IEnumerable<string> ids)
        {
            if (ids.Any())
            {
                var queryResult = await Mediator
                    .Send(new DbListRequest<Actor>(ids))
                    .ConfigureAwait(false);
                if (queryResult.Success)
                {
                    return queryResult.Entities;
                }
            }
            return new List<Actor>();
        }

        private async Task<IEnumerable<string>> GetGeosIdsAsync
            (IEnumerable<string> ids)
        {
            if (ids.Any())
            {
                var queryResult = await Mediator
                    .Send(new DbListRequest<Geo>(ids))
                    .ConfigureAwait(false);
                if (queryResult.Success)
                {
                    return queryResult.Entities.Select(x => x.Id);
                }
            }
            return new List<string>();
        }

        private async Task<ValidationResult> CheckPermissionAsync
            (GeoTask oldTask, GeoTask newTask, Actor currentActor,
             ActorRole oldProjectRole, ActorRole newProjectRole)
        {
            var checkModel = new CheckUpdatePermissionModel<GeoTask>
            {
                Actor = currentActor,
                OldProjectRole = oldProjectRole,
                NewProjectRole = newProjectRole,
                EntityBeforeUpdate = oldTask,
                EntityAfterUpdate = newTask
            };
            var validator = new GeoTaskUpdatePermissionValidator();
            var validatorResult = await validator
                .ValidateAsync(checkModel)
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
