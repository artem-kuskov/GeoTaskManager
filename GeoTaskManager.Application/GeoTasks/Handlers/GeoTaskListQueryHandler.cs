using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.DbQueries;
using GeoTaskManager.Application.GeoTasks.Mappers;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Queries;
using GeoTaskManager.Application.GeoTasks.Security;
using GeoTaskManager.Application.GeoTasks.Validators;
using GeoTaskManager.Application.Projects.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.GeoTasks.Handlers
{
    public class GeoTaskListQueryHandler
        : IRequestHandler<GeoTaskListQuery, ListResponse<GeoTask>>
    {
        private ILogger<GeoTaskListQueryHandler> Logger { get; }
        private IConfiguration Configuration { get; }
        private IMediator Mediator { get; }

        public GeoTaskListQueryHandler(IMediator mediator,
            ILogger<GeoTaskListQueryHandler> logger,
            IConfiguration configuration)
        {
            Mediator = mediator;
            Logger = logger;
            Configuration = configuration;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<GeoTask>> Handle
            (GeoTaskListQuery request, CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle get task collection {request}",
                request.ToDictionary());

            if (request is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle get task collection request with empty request");
                return ErrorResponse("Request empty argument");
            }

            try
            {
                // Validate request
                var validator = new GeoTaskListQueryValidator();
                var validationResult = await validator.ValidateAsync(request)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogError("Query validation error. " +
                        "Request={Request}. Error={Error}.",
                        request.ToDictionary(),
                        validationResult.Errors.Select(x => x.ErrorMessage));
                    return ErrorResponse
                        (validationResult.Errors.Select(x => x.ErrorMessage));
                }

                // Build filter
                var defaultLimit = Configuration.GetValue
                    (Defaults.ConfigurationApiDefaultLimitParameterName,
                    Defaults.ConfigurationApiDefaultLimitDefaultValue);
                var maxLimit = Configuration.GetValue
                    (Defaults.ConfigurationApiMaxLimitParameterName,
                    Defaults.ConfigurationApiMaxLimitDefaultValue);
                var filter = new DbGetGeoTaskListRequest
                {
                    Offset = request.Offset,
                    Limit = request.Limit == 0
                        ? defaultLimit
                        : Math.Min(request.Limit, maxLimit),
                    Archived = request.Archived,
                    Contains = request.СontainsKeyWords,
                    MaxTimeToDeadLine = request.MaxTimeToDeadLine,
                    TaskStatusMask = request.TaskStatusMask,
                };
                if (!String.IsNullOrWhiteSpace(request.ActorId))
                {
                    filter.Actors[request.ActorId] = request.ActorRoleMask;
                }
                if (!string.IsNullOrWhiteSpace(request.ProjectId))
                {
                    filter.ProjectIds.Add(request.ProjectId);
                }
                filter.GeoIds.AddRange(request.GeoIds);

                // Get Actor for current user by user name
                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (request.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                Actor currentActor = null;
                if (currentActorResponse.Success)
                {
                    currentActor = currentActorResponse.Entity;
                }
                else
                {
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Not found current actor");
                    return ErrorResponse("Not found current actor");
                }

                ActorRole projectActorRole = null;
                if (!string.IsNullOrWhiteSpace(request.ProjectId))
                {
                    projectActorRole = await GetProjectRoleAsync
                        (request.ProjectId, currentActor?.Id)
                        .ConfigureAwait(false);
                }

                // Apply security filter
                var securizator = new DbGetGeoTaskListRequestSecurityBuilder
                    (filter, currentActor, projectActorRole);
                var securedFilter = securizator.Build();

                return await Mediator.Send(securedFilter)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResponse("Not found");
            }
        }

        private async Task<ActorRole> GetProjectRoleAsync(string projectId,
            string actorId)
        {
            if (string.IsNullOrWhiteSpace(projectId)
                || string.IsNullOrWhiteSpace(actorId))
            {
                return null;
            }
            var projectRequest = await Mediator
                .Send(new DbGetEntityByIdRequest<Project>(projectId))
                .ConfigureAwait(false);
            if (!projectRequest.Success)
            {
                return null;
            }
            return projectRequest.Entity.ProjectActorRoles
                .FirstOrDefault(x => x.Key == actorId).Value;
        }

        private ListResponse<GeoTask> ErrorResponse(string error)
        {
            var errorResult = new ListResponse<GeoTask> { Success = false };
            errorResult.Errors.Add(error);
            return errorResult;
        }

        private ListResponse<GeoTask> ErrorResponse(IEnumerable<string> error)
        {
            var errorResult = new ListResponse<GeoTask> { Success = false };
            errorResult.Errors.AddRange(error);
            return errorResult;
        }
    }
}
