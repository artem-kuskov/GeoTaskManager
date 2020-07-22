using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.Geos.Validators;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;
using _TListQuery = GeoTaskManager.Application.Geos.Queries.GeoListQuery;


namespace GeoTaskManager.Application.Geos.Handlers
{
    public class GeoListQueryHandler
        : IRequestHandler<_TListQuery, ListResponse<_TEntity>>
    {
        private IMediator Mediator { get; }
        private ILogger<GeoListQueryHandler> Logger { get; }
        private IConfiguration Configuration { get; }

        public GeoListQueryHandler(IMediator mediator,
            ILogger<GeoListQueryHandler> logger,
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
                "Handle Get Geo List {Request}", request.ToDictionary());

            if (request is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Get Geo List request with empty request");
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

                // Build request
                var defaultLimit = Configuration.GetValue
                    (Defaults.ConfigurationApiDefaultLimitParameterName,
                    Defaults.ConfigurationApiDefaultLimitDefaultValue);
                var maxLimit = Configuration.GetValue
                    (Defaults.ConfigurationApiMaxLimitParameterName,
                    Defaults.ConfigurationApiMaxLimitDefaultValue);

                var dbGeoListRequest = request
                    .ToDbGetGeoFilterRequest(defaultLimit, maxLimit);

                // Check current actor
                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (request.CurrentPrincipal?.Identity?.Name))
                    .ConfigureAwait(false);
                if (!currentActorResponse.Success
                    || currentActorResponse.Entity.IsArchived)
                {
                    Logger.LogWarning(AppLogEvent.HandleArgumentError,
                        "Not found current actor");
                    return ErrorResponse("Not found current actor");
                }

                return await Mediator
                    .Send(dbGeoListRequest)
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
