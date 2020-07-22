using GeoTaskManager.Application.Configuration;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.Events;
using GeoTaskManager.Application.Geos.Mappers;
using GeoTaskManager.Application.GeoTasks.DbQueries;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.Application.GeoTasks.Handlers
{
    public class AfterEntityDeleteProjectHandler
        : INotificationHandler<AfterEntityDelete<Project>>
    {
        private IMediator Mediator { get; }
        private ILogger<AfterEntityDeleteProjectHandler> Logger { get; }

        public AfterEntityDeleteProjectHandler
            (IMediator mediator,
            ILogger<AfterEntityDeleteProjectHandler> logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task Handle(AfterEntityDelete<Project> notification,
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
                        "After Project Delete Event Handler. " +
                        "Notification={Notification}.",
                        notification.Entity.ToDictionary());

                var taskResponse = await Mediator
                    .Send(new DbGetGeoTaskListRequest
                    {
                        ProjectIds = { notification.Entity.Id }
                    })
                    .ConfigureAwait(false);

                if (taskResponse.Success)
                {
                    await Mediator
                        .Send(new DbDeleteListCommand<GeoTask>
                            (taskResponse.Entities.Select(x => x.Id).ToList()))
                        .ConfigureAwait(false);
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
