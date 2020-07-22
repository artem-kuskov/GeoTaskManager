using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Commands;
using System.Linq;
using System.Security.Claims;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class ApiGeoTaskExtensions
    {
        public static GeoTaskUpdateCommand ToGeoTaskUpdateCommand
            (this ApiGeoTask from, string id, ClaimsPrincipal currentPrincipal,
            string messageTitle, string messageDescription)
        {
            if (from is null)
            {
                return null;
            }

            var to = new GeoTaskUpdateCommand
            {
                Description = from.Description,
                Id = id,
                PlanFinishAt = from.PlanFinishAt,
                PlanStartAt = from.PlanStartAt,
                ProjectId = from.ProjectId,
                ResponsibleActorId = from.ResponsibleActor?.Id,
                Title = from.Title,
                CurrentPrincipal = currentPrincipal,
                IsArchived = from.IsArchived,
                Status = from.Status,
                MessageDescription = messageDescription,
                MessageTitle = messageTitle
            };
            to.AssistentActorsIds
                .AddRange(from.AssistentActors.Select(x => x.Id));
            to.GeosIds.AddRange(from.GeosIds);
            to.ObserverActorsIds
                .AddRange(from.ObserverActors.Select(x => x.Id));
            return to;
        }
    }
}
