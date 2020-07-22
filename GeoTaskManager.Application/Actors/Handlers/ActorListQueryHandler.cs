using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Actors.Queries;
using GeoTaskManager.Application.Actors.Validators;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Mappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Actors.Handlers
{
    public class ActorListQueryHandler
        : IRequestHandler<ActorListQuery, ListResponse<Actor>>
    {
        private ILogger<ActorListQueryHandler> Logger { get; }
        private IConfiguration Configuration { get; }
        private IMediator Mediator { get; }

        public ActorListQueryHandler(IGeoTaskManagerDbContext dbContext,
            ILogger<ActorListQueryHandler> logger,
            IConfiguration configuration, IMediator mediator)
        {
            Logger = logger;
            Configuration = configuration;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<Actor>> Handle(ActorListQuery request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Get Actor List {request}", request.ToDictionary());

            if (request is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle get task collection request with empty request");
                return ErrorResponse("Request empty argument");
            }

            try
            {
                // Validate request
                var validator = new ActorListQueryValidator();
                var validationResult = await validator.ValidateAsync(request)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogError("Query validation error. " +
                        "Request = {request}. Error = {Error}",
                        request.ToDictionary(),
                        validationResult.Errors.Select(x => x.ErrorMessage));
                    return ErrorResponse
                        (validationResult.Errors.Select(x => x.ErrorMessage));
                }

                // Get Actor for current user by user name
                var currentActorRequest =
                    new DbGetActorByNameRequest
                        (request.CurrentPrincipal?.Identity?.Name);
                var currentActorResponse = await Mediator
                    .Send(currentActorRequest)
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

                var defaultLimit = Configuration.GetValue
                    (Defaults.ConfigurationApiDefaultLimitParameterName,
                    Defaults.ConfigurationApiDefaultLimitDefaultValue);
                var maxLimit = Configuration.GetValue
                    (Defaults.ConfigurationApiMaxLimitParameterName,
                    Defaults.ConfigurationApiMaxLimitDefaultValue);
                var dbRequest = request.ToDbGetActorFilterRequest(defaultLimit,
                    maxLimit);
                var dbResponse = await Mediator.Send(dbRequest)
                                               .ConfigureAwait(false);
                return dbResponse;
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResponse("Not found");
            }
        }

        private ListResponse<Actor> ErrorResponse(string error)
        {
            var errorResult = new ListResponse<Actor> { Success = false };
            errorResult.Errors.Add(error);
            return errorResult;
        }

        private ListResponse<Actor> ErrorResponse(IEnumerable<string> error)
        {
            var errorResult = new ListResponse<Actor> { Success = false };
            errorResult.Errors.AddRange(error);
            return errorResult;
        }
    }
}
