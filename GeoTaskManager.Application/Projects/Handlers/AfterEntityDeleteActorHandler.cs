using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.Events;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.Projects.DbQueries;
using GeoTaskManager.Application.Projects.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Projects.Handlers
{
    public class AfterEntityDeleteActorHandler
        : INotificationHandler<AfterEntityDelete<Actor>>
    {
        private IMediator Mediator { get; }
        private ILogger<AfterEntityDeleteActorHandler> Logger { get; }

        public AfterEntityDeleteActorHandler
            (IMediator mediator,
            ILogger<AfterEntityDeleteActorHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task Handle(AfterEntityDelete<Actor> notification,
            CancellationToken cancellationToken)
        {
            if (notification is null || notification.Entity is null)
            {
                Logger.LogWarning(AppLogEvent.HandleArgumentError,
                    "Entity Delete empty notification");
                throw new ArgumentNullException
                    (notification is null
                        ? nameof(notification)
                        : nameof(notification.Entity));
            }

            try
            {
                Logger.LogInformation(AppLogEvent.HandleRequest,
                        "After Actor Delete Event Handler. " +
                        "Notification={Notification}.",
                        notification.Entity.ToDictionary());

                var projectResponse = await Mediator
                    .Send(new DbGetProjectFilterRequest())
                    .ConfigureAwait(false);

                if (projectResponse.Success)
                {
                    foreach (var item in projectResponse.Entities)
                    {
                        if (item.ProjectActorRoles
                                .ContainsKey(notification.Entity.Id))
                        {
                            item.ProjectActorRoles
                                .Remove(notification.Entity.Id);
                        }
                        await Mediator
                            .Send(new DbUpdateCommand<Project>(item))
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(AppLogEvent.HandleErrorResponse, e,
                    "Call repository exception");
                throw;
            }
        }
    }
}
