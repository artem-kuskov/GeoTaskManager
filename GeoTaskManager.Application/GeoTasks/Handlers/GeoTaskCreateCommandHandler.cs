using FluentValidation;
using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
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
    public class GeoTaskCreateCommandHandler
        : IRequestHandler<GeoTaskCreateCommand, CreateResult>
    {
        private const Microsoft.AspNetCore.JsonPatch.Operations.OperationType addOperation = Microsoft.AspNetCore.JsonPatch.Operations.OperationType.Add;

        private IMediator Mediator { get; }
        private ILogger<GeoTaskCreateCommandHandler> Logger { get; }

        public GeoTaskCreateCommandHandler(IMediator mediator,
            ILogger<GeoTaskCreateCommandHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle(GeoTaskCreateCommand command,
            CancellationToken cancellationToken)
        {
            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle create task got empty command");
                return ErrorResult("Empty Geo Task Create command");
            }

            var validator = new GeoTaskCreateCommandValidator();
            var validationResult = await validator
                .ValidateAsync(command)
                .ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                Logger.LogWarning(AppLogEvent.RequestValidationError,
                    "Validation command error. Command = {command}. " +
                    "Error = {Error}", command.ToDictionary(),
                    validationResult.Errors);
                return ErrorResult
                    (validationResult.Errors.Select(x => x.ErrorMessage));
            }

            try
            {
                var geoTask = new GeoTask()
                {
                    CreatedAt = DateTime.UtcNow,
                    Description = command.Description,
                    StatusChangedAt = DateTime.UtcNow,
                    IsArchived = command.IsArchived,
                    PlanFinishAt = command.PlanFinishAt,
                    PlanStartAt = command.PlanStartAt,
                    Status = GeoTaskStatus.New,
                    Title = command.Title,
                };

                // Load from repository all mentioned actors
                var allActors = await GetActors(
                    command.AssistentActorsIds
                        .Concat(command.ObserverActorsIds)
                        .Append(command.ResponsibleActorId)
                    ).ConfigureAwait(false);

                // Add to the new entity only exist in the repository actors
                geoTask.AssistentActors.AddRange(
                    command.AssistentActorsIds
                        .Select(x => allActors
                            .FirstOrDefault(a => a.Id == x))
                        .Where(a => a != null));
                geoTask.ObserverActors.AddRange(
                    command.ObserverActorsIds
                        .Select(x => allActors
                            .FirstOrDefault(a => a.Id == x))
                        .Where(a => a != null));
                geoTask.ResponsibleActor = allActors
                    .FirstOrDefault(a => a.Id == command.ResponsibleActorId);

                // Check that Geo Ids exist in repository
                geoTask.GeosIds.AddRange(await GetGeosIds(command.GeosIds)
                                                       .ConfigureAwait(false));

                // Get Actor for current user by user name
                var creatorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (command.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                Actor createdBy = null;
                if (creatorResponse.Success)
                {
                    createdBy = creatorResponse.Entity;
                }

                geoTask.CreatedBy = createdBy;

                Project project = await GetProject(command.ProjectId)
                    .ConfigureAwait(false);
                geoTask.ProjectId = project?.Id;

                var historyRec = new GeoTaskHistory
                {
                    ChangedAt = DateTime.UtcNow,
                    ChangedBy = createdBy,
                };
                historyRec.Operations.Add(new Operation
                {
                    OperationType = addOperation,
                    Path = "/",
                    NewValue = geoTask.ToDictionary()
                });
                geoTask.History.Add(historyRec);

                var validatorBeforeSave = new GeoTaskBeforeSaveValidator();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(geoTask)
                    .ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestNotValid,
                        "GeoTask validation error. Entity={Entity}. " +
                        "Error={Error}.", geoTask.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                        .Select(x => x.ErrorMessage));
                }

                var checkPermissionResult = await CheckPermission
                        (geoTask, createdBy, project)
                    .ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "GeoTask check create permission error. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}. " +
                        "Project={Project}. Error={Error}.",
                        geoTask.ToDictionary(), createdBy?.ToDictionary(),
                        project?.ToDictionary(), checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                return await Mediator
                    .Send(new DbCreateCommand<GeoTask>(geoTask))
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResult("Not found");
            }
        }

        private async Task<Project> GetProject(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
            {
                return null;
            }
            var projectRequest = await Mediator
                .Send(new DbGetEntityByIdRequest<Project>(projectId))
                .ConfigureAwait(false);
            return !projectRequest.Success ? null : projectRequest.Entity;
        }

        private async Task<CreateResult> CheckPermission(GeoTask task,
            Actor actor, Project project)
        {
            ActorRole currentActorProjectRole = null;
            if (project != null && actor != null)
            {
                project.ProjectActorRoles?
                    .TryGetValue(actor.Id, out currentActorProjectRole);
            }
            var checkModel = new CheckCreatePermissionModel<GeoTask>
            {
                Entity = task,
                Actor = actor,
                ProjectActorRole = currentActorProjectRole
            };
            var validator = new GeoTaskCreatePermissionValidator();
            var validatorResult = await validator
                .ValidateAsync(checkModel)
                .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult(validatorResult.Errors
                    .Select(x => x.ErrorMessage));
            }
            return new CreateResult { Success = true, Id = task.Id };
        }

        private async Task<IEnumerable<Actor>> GetActors
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

        private async Task<IEnumerable<string>> GetGeosIds
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

        private CreateResult ErrorResult(IEnumerable<string> errors)
        {
            var result = new CreateResult { Success = false };
            result.Errors.AddRange(errors);
            return result;
        }

        private CreateResult ErrorResult(string error)
            => ErrorResult(new string[] { error });
    }
}
