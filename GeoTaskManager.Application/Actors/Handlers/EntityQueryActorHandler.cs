using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Actors.Validators;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Queries;
using GeoTaskManager.Application.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Actors.Handlers
{
    public class EntityQueryActorHandler
        : IRequestHandler<EntityQuery<Actor>, EntityResponse<Actor>>
    {
        private ILogger<EntityQueryActorHandler> Logger { get; }
        private IMediator Mediator { get; }

        public EntityQueryActorHandler(IMediator mediator,
            ILogger<EntityQueryActorHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<Actor>> Handle
            (EntityQuery<Actor> request, CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Entity Query (Actor) {Request}",
                request.ToDictionary());

            if (request is null || string.IsNullOrWhiteSpace(request.Id))
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Entity Query (Actor) request with empty request");
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
                    .Send(new DbGetEntityByIdRequest<Actor>(request.Id))
                    .ConfigureAwait(false);

                if (!getResponse.Success)
                {
                    Logger.LogWarning(AppLogEvent.HandleNullResponse,
                        "Handle Entity Query (Actor) can not get entity by id");
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
                        "Handle Get Actor permission error. " +
                        "Entity = {Entity}. CurrentActor={CurrentActor}. " +
                        "Error = {Error}.",
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

        private async Task<EntityResponse<Actor>> CheckPermission
            (Actor actor, Actor currentActor, ClaimsPrincipal currentPrincipal)
        {
            var checkModel = new CheckQueryPermissionModel<Actor>
            {
                Entity = actor,
                Actor = currentActor,
                Principal = currentPrincipal
            };

            var validator = new EntityQueryActorPermissionValidator();
            var validatorResult = await validator
                .ValidateAsync(checkModel)
                .ConfigureAwait(false);

            if (!validatorResult.IsValid)
            {
                var errors = validatorResult.Errors
                    .Select(x => x.ErrorMessage);
                return ErrorResult(errors);
            }

            return new EntityResponse<Actor>
            {
                Success = true,
                Entity = actor
            };
        }

        private EntityResponse<Actor> ErrorResult(string error)
        {
            return ErrorResult(new string[] { error });
        }

        private EntityResponse<Actor> ErrorResult(IEnumerable<string> errors)
        {
            var errorResult = new EntityResponse<Actor> { Success = false };
            errorResult.Errors.AddRange(errors);
            return errorResult;
        }
    }
}
