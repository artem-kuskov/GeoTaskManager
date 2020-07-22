using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Events;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.Application.Projects.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.Projects.Handlers
{
    public class AfterEntityDeleteGeoHandler
        : INotificationHandler<AfterEntityDelete<Geo>>
    {
        private IMediator Mediator { get; }
        private ILogger<AfterEntityDeleteGeoHandler> Logger { get; }

        public AfterEntityDeleteGeoHandler
            (IMediator mediator,
            ILogger<AfterEntityDeleteGeoHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task Handle(AfterEntityDelete<Geo> notification,
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
                        "After Geo Delete Event Handler. " +
                        "Notification={Notification}.",
                        notification.Entity.ToDictionary());

                var projectResponse = await Mediator
                    .Send(new DbGetEntityByIdRequest<Project>
                        (notification.Entity.ProjectId))
                    .ConfigureAwait(false);

                if (projectResponse.Success)
                {
                    var entityChanged = false;
                    foreach (var item in projectResponse.Entity.Layers)
                    {
                        if (item.GeoId == notification.Entity.Id)
                        {
                            item.GeoId = null;
                            entityChanged = true;
                        }
                    }

                    if (entityChanged)
                    {
                        await Mediator
                            .Send(new DbUpdateCommand<Project>
                                (projectResponse.Entity))
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
