using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _TCreateCommand = GeoTaskManager.Application.Projects.Commands.ProjectCreateCommand;
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;


namespace GeoTaskManager.Application.Projects.Handlers
{
    public class ProjectCreateCommandHandler
        : IRequestHandler<_TCreateCommand, CreateResult>
    {
        private IMediator Mediator { get; }
        private ILogger<ProjectCreateCommandHandler> Logger { get; }

        public ProjectCreateCommandHandler(IMediator mediator,
            ILogger<ProjectCreateCommandHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle(_TCreateCommand command,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation(AppLogEvent.HandleRequest,
                "Handle Project Create Command. Command={Command}",
                command.ToDictionary());

            if (command is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Handle Create Project Command got empty command");
                return ErrorResult("Empty Project Create command");
            }

            try
            {
                var validator = new CreateCommandValidator<_TCreateCommand>();
                var validationResult = await validator.ValidateAsync(command)
                    .ConfigureAwait(false);
                if (!validationResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestValidationError,
                        "Validation command error. Command={Command}. " +
                        "Error={Error}.",
                        command.ToDictionary(), validationResult.Errors);
                    return ErrorResult(validationResult.Errors
                                        .Select(x => x.ErrorMessage));
                }

                // Get Actor for current user by user name
                Actor createdBy = null;
                var currentUserName = command.CurrentPrincipal?
                                            .Identity?
                                            .Name;
                var creatorResponse = await Mediator
                    .Send(new DbGetActorByNameRequest(currentUserName))
                    .ConfigureAwait(false);
                if (creatorResponse.Success)
                {
                    createdBy = creatorResponse.Entity;
                    //TODO Add to all create permission validation CreatedBy not null check
                }

                var entity = new _TEntity()
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    Description = command.Description,
                    IsArchived = command.IsArchived,
                    Title = command.Title,
                    IsMap = command.IsMap,
                    MapProvider = command.MapProvider,
                    ShowMap = command.ShowMap
                };
                entity.Layers.AddRange(command.Layers);
                command.MapParameters
                    .ToList()
                    .ForEach(x => entity.MapParameters.TryAdd(x.Key, x.Value));
                var checkingActors = command.ProjectActorRoles.Keys;
                if (checkingActors.Count > 0)
                {
                    var actorIdsFromRepository = await GetActorIds
                        (checkingActors).ConfigureAwait(false);
                    foreach (var item in command.ProjectActorRoles)
                    {
                        if (!String.IsNullOrWhiteSpace(item.Key)
                            && actorIdsFromRepository.Contains(item.Key))
                        {
                            entity.ProjectActorRoles.Add(item.Key, item.Value);
                        }
                    }
                }

                var validatorBeforeSave = new BeforeSaveValidator<_TEntity>();
                var validationBeforeSaveResult = await validatorBeforeSave
                    .ValidateAsync(entity).ConfigureAwait(false);
                if (!validationBeforeSaveResult.IsValid)
                {
                    Logger.LogWarning(AppLogEvent.RequestNotValid,
                        "Project validation error. Entity={Entity}. " +
                        "Error={Error}.",
                        entity.ToDictionary(),
                        validationBeforeSaveResult.Errors);
                    return ErrorResult(validationBeforeSaveResult.Errors
                                        .Select(x => x.ErrorMessage));
                }

                var checkPermissionResult = await CheckPermission(entity,
                    createdBy).ConfigureAwait(false);
                if (!checkPermissionResult.Success)
                {
                    Logger.LogWarning(AppLogEvent.SecurityNotPassed,
                        "Project Create permission error. " +
                        "Entity={Entity}. CurrentActor={CurrentActor}." +
                        " Error={Error}.",
                        entity.ToDictionary(), createdBy?.ToDictionary(),
                        checkPermissionResult.Errors);
                    return checkPermissionResult;
                }

                return await Mediator.Send
                        (new DbCreateCommand<_TEntity>(entity))
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                return ErrorResult("Not found");
            }
        }

        private async Task<HashSet<string>> GetActorIds
            (IEnumerable<string> checkingActors)
        {
            var respond = await Mediator
                .Send(new DbListRequest<Actor>(checkingActors))
                .ConfigureAwait(false);
            return respond.Success
                ? respond.Entities.Select(x => x.Id).ToHashSet()
                : new HashSet<string>();
        }

        private async Task<CreateResult> CheckPermission
            (_TEntity entity, Actor currentActor)
        {
            var checkModel = new CheckCreatePermissionModel<_TEntity>
            {
                Entity = entity,
                Actor = currentActor
            };
            var validator = new CreatePermissionValidator<_TEntity>();
            var validatorResult = await validator.ValidateAsync(checkModel)
                                                 .ConfigureAwait(false);
            if (!validatorResult.IsValid)
            {
                return ErrorResult(validatorResult.Errors
                                        .Select(x => x.ErrorMessage));
            }
            return new CreateResult { Success = true };
        }

        private CreateResult ErrorResult(IEnumerable<string> errors)
        {
            var result = new CreateResult { Success = false };
            result.Errors.AddRange(errors);
            return result;
        }

        private CreateResult ErrorResult(string error)
        {
            return ErrorResult(new string[] { error });
        }
    }
}
