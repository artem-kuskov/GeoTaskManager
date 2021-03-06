﻿using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Actors.Validators;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Events;
using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Actors.Handlers
{
    public class DeleteCommandActorHandler
        : IRequestHandler<DeleteCommand<Actor>, DeleteResult>
    {
        private ILogger<DeleteCommandActorHandler> Logger { get; }
        private IMediator Mediator { get; }

        public DeleteCommandActorHandler
            (ILogger<DeleteCommandActorHandler> logger, IMediator mediator)
        {
            Logger = logger;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<DeleteResult> Handle(DeleteCommand<Actor> command,
            CancellationToken cancellationToken)
        {
            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Actor Delete empty command");
                return ErrorResult("Actor Delete empty command");
            }

            try
            {
                var validator = new ActorDeleteCommandValidator();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Validation command error. Command={Command}. " +
                        "Error={Error}.",
                        command.ToDictionary(), validationResult.Errors);
                    return ErrorResult
                        (validationResult.Errors.Select(x => x.ErrorMessage));
                }

                var entityResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Actor>(command.Id))
                    .ConfigureAwait(false);
                if (entityResponse.Success != true)
                {
                    Logger.LogWarning(AppLogEvent.HandleNullResponse,
                        "Handle Actor Delete Command can not get the actor. " +
                        "Command={Command}. Error={Error}.",
                        command.ToDictionary(), entityResponse.Errors);
                    return ErrorResult(entityResponse.Errors);
                }
                var entity = entityResponse.Entity;

                // Get Actor for current user by user name
                var currentActorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest
                        (command.CurrentPrincipal.Identity?.Name))
                    .ConfigureAwait(false);
                Actor currentActor = null;
                if (currentActorResponse.Success)
                {
                    currentActor = currentActorResponse.Entity;
                }

                var checkPermissionResult = await CheckPermission
                    (entity, currentActor, command.HardMode)
                    .ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Actor Delete permission not passed. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}. " +
                        "Error={Error}.",
                        entity.ToDictionary(), currentActor?.ToDictionary(),
                        checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                if (command.HardMode)
                {
                    await Mediator.Publish
                        (new BeforeEntityDelete<Actor>(entity))
                        .ConfigureAwait(false);

                    var result = await Mediator
                        .Send(new DbDeleteCommand<Actor>(entity.Id))
                        .ConfigureAwait(false);

                    await Mediator.Publish
                        (new AfterEntityDelete<Actor>(entity))
                        .ConfigureAwait(false);

                    return result;
                }
                else
                {
                    return await SoftDelete(entity)
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

        private async Task<DeleteResult> SoftDelete
            (Actor entity)
        {
            entity.IsArchived = true;
            var updateResult = await Mediator
                .Send(new DbUpdateCommand<Actor>(entity))
                .ConfigureAwait(false);
            return updateResult.ToDeleteResult();
        }

        private async Task<DeleteResult> CheckPermission(Actor entity,
            Actor currentActor, bool hardMode)
        {
            var checkModel = new CheckDeletePermissionModel<Actor>
            {
                Entity = entity,
                Actor = currentActor,
                HardMode = hardMode
            };
            var validator = new ActorDeletePermissionValidator();
            var validatorResult = await validator.ValidateAsync(checkModel)
                                        .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult
                    (validatorResult.Errors.Select(x => x.ErrorMessage));
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
            => ErrorResult(new string[] { error });
    }
}
