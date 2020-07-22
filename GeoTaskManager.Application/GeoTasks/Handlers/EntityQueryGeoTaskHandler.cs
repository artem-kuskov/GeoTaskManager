using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Queries;
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

namespace GeoTaskManager.Application.GeoTasks.Handlers
{
    public class EntityQueryGeoTaskHandler
        : IRequestHandler<EntityQuery<GeoTask>, EntityResponse<GeoTask>>
    {
        private IMediator Mediator { get; }
        private ILogger<EntityQueryGeoTaskHandler> Logger { get; }

        public EntityQueryGeoTaskHandler(IMediator mediator,
            ILogger<EntityQueryGeoTaskHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<GeoTask>> Handle
            (EntityQuery<GeoTask> request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle get task {Request}", request.ToDictionary());

            if (request is null || string.IsNullOrWhiteSpace(request.Id))
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle get task request with empty request");
                return ErrorResult("Request empty argument");
            }

            try
            {
                var getResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<GeoTask>(request.Id))
                    .ConfigureAwait(false);
                if (!getResponse.Success)
                {
                    Logger.LogWarning(AppLogEvent.HandleNullResponse,
                        "Handle get task can not get task by id");
                    return getResponse;
                }
                var task = getResponse.Entity;

                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (request.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                var currentActor = currentActorResponse.Entity;

                var projectResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Project>(task.ProjectId))
                    .ConfigureAwait(false);
                var project = projectResponse.Entity;

                // Validate permission
                var checkPermissionResult = await CheckPermission
                    (task, currentActor, project).ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "GeoTask check get permission error. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}. " +
                        "Project={Project}. Error={Error}.",
                        task.ToDictionary(), currentActor?.ToDictionary(),
                        project?.ToDictionary(), checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                return getResponse;
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResult("Not found");
            }
        }

        private async Task<EntityResponse<GeoTask>> CheckPermission
            (GeoTask task, Actor actor, Project project)
        {
            ActorRole currentActorProjectRole = null;
            if (project != null && actor != null)
            {
                project.ProjectActorRoles.TryGetValue(actor.Id,
                    out currentActorProjectRole);
            }
            var checkModel = new CheckQueryPermissionModel<GeoTask>
            {
                Entity = task,
                Actor = actor,
                ProjectActorRole = currentActorProjectRole
            };
            var validator = new GeoTaskQueryPermissionValidator();
            var validatorResult = await validator.ValidateAsync(checkModel)
                .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult(validatorResult.Errors
                    .Select(x => x.ErrorMessage));
            }
            return new EntityResponse<GeoTask>
            {
                Success = true,
                Entity = task
            };
        }

        private EntityResponse<GeoTask> ErrorResult(string error)
            => ErrorResult(new string[] { error });

        private EntityResponse<GeoTask> ErrorResult(IEnumerable<string> errors)
        {
            var errorResult = new EntityResponse<GeoTask> { Success = false };
            errorResult.Errors.AddRange(errors);
            return errorResult;
        }
    }
}
