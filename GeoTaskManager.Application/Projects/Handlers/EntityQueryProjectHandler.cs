using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _EntityType = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Application.Projects.Handlers
{
    public class EntityQueryProjectHandler
        : IRequestHandler<EntityQuery<_EntityType>,
            EntityResponse<_EntityType>>
    {
        private IMediator Mediator { get; }
        private ILogger<EntityQueryProjectHandler> Logger { get; }

        public EntityQueryProjectHandler(IMediator mediator,
            ILogger<EntityQueryProjectHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<_EntityType>> Handle
            (EntityQuery<_EntityType> request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Get Project {request}", request.ToDictionary());

            if (request is null || string.IsNullOrWhiteSpace(request.Id))
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Get Project request with empty request");
                return ErrorResult("Request empty argument");
            }

            try
            {
                var currentUserName = request.CurrentPrincipal?.Identity?.Name;
                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest(currentUserName))
                    .ConfigureAwait(false);
                var currentActor = currentActorResponse.Success
                    ? currentActorResponse.Entity
                    : null;

                var getResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<_EntityType>(request.Id))
                    .ConfigureAwait(false);

                if (!getResponse.Success)
                {
                    Logger.LogWarning(AppLogEvent.HandleNullResponse,
                        "Handle Get Entity can not get entity by id");
                    return getResponse;
                }

                var entity = getResponse.Entity;

                // Validate permission
                var checkPermissionResult = await CheckPermission
                    (entity, currentActor, request.CurrentPrincipal)
                    .ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Handle Get Entity permission error. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}. " +
                        "Error={Error}.",
                        entity.ToDictionary(), currentActor?.ToDictionary(),
                        checkPermissionResult.Errors);
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

        private async Task<EntityResponse<_EntityType>> CheckPermission
            (_EntityType entity, Actor currentActor,
            ClaimsPrincipal currentPrincipal)
        {
            var checkModel = new CheckQueryPermissionModel<_EntityType>
            {
                Entity = entity,
                Actor = currentActor,
                Principal = currentPrincipal
            };

            var validator = new EntityQueryPermissionValidator<_EntityType>();
            var validatorResult = await validator
                .ValidateAsync(checkModel)
                .ConfigureAwait(false);

            if (!validatorResult.IsValid)
            {
                var errors = validatorResult.Errors
                    .Select(x => x.ErrorMessage);
                return ErrorResult(errors);
            }

            return new EntityResponse<_EntityType>
            {
                Success = true,
                Entity = entity
            };
        }

        private EntityResponse<_EntityType> ErrorResult(string error)
        {
            return ErrorResult(new string[] { error });
        }

        private EntityResponse<_EntityType> ErrorResult
            (IEnumerable<string> errors)
        {
            var errorResult = new EntityResponse<_EntityType>
            {
                Success = false
            };
            errorResult.Errors.AddRange(errors);
            return errorResult;
        }
    }
}
