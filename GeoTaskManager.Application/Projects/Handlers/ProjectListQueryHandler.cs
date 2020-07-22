using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.DbQueries;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Security;
using GeoTaskManager.Application.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;
using _TListQuery = GeoTaskManager.Application.Projects.Queries.ProjectListQuery;


namespace GeoTaskManager.Application.Projects.Handlers
{
    public class ProjectListQueryHandler
        : IRequestHandler<_TListQuery, ListResponse<_TEntity>>
    {
        private IMediator Mediator { get; }
        private ILogger<ProjectListQueryHandler> Logger { get; }
        private IConfiguration Configuration { get; }

        public ProjectListQueryHandler(IMediator mediator,
            ILogger<ProjectListQueryHandler> logger,
            IConfiguration configuration)
        {
            Mediator = mediator;
            Logger = logger;
            Configuration = configuration;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<_TEntity>> Handle(_TListQuery request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Get Project List {Request}", request.ToDictionary());

            if (request is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Get Project List request with empty request");
                return ErrorResponse("Request empty argument");
            }

            try
            {
                // Validate request
                var validator = new ListQueryValidator<_TListQuery>();
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
                var dbRequest = new DbGetProjectFilterRequest
                {
                    Offset = request.Offset,
                    Limit = request.Limit == 0
                        ? defaultLimit
                        : Math.Min(request.Limit, maxLimit),
                    Archived = request.Archived,
                    Contains = request.СontainsKeyWords
                };

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

                // Apply security filter
                var securizator = new DbGetProjectFilterRequestSecurityBuilder
                    (dbRequest, currentActor);
                DbGetProjectFilterRequest securedFilter = securizator.Build();

                return await Mediator
                    .Send(securedFilter)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResponse("Not found");
            }
        }

        private ListResponse<_TEntity> ErrorResponse(string error)
        {
            var errorResult = new ListResponse<_TEntity> { Success = false };
            errorResult.Errors.Add(error);
            return errorResult;
        }

        private ListResponse<_TEntity> ErrorResponse(IEnumerable<string> error)
        {
            var errorResult = new ListResponse<_TEntity> { Success = false };
            errorResult.Errors.AddRange(error);
            return errorResult;
        }
    }
}
