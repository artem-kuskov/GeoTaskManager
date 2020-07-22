using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Commands;
using System.Collections.Generic;
using System.Security.Claims;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class ApiGeoTaskCreateCommandExtensions
    {
        public static GeoTaskCreateCommand ToAppGeoTaskCreateCommand
            (this ApiGeoTaskCreateCommand from,
            ClaimsPrincipal currentPrincipal)
        {
            if (from is null)
            {
                return null;
            }

            var to = new GeoTaskCreateCommand
            {
                Description = from.Description,
                PlanFinishAt = from.PlanFinishAt,
                PlanStartAt = from.PlanStartAt,
                ProjectId = from.ProjectId,
                ResponsibleActorId = from.ResponsibleActorId,
                Title = from.Title,
                CurrentPrincipal = currentPrincipal,
            };
            to.AssistentActorsIds.AddRange(from.AssistentActorsIds);
            to.GeosIds.AddRange(from.GeosIds);
            to.ObserverActorsIds.AddRange(from.ObserverActorsIds);

            return to;
        }

        public static Dictionary<string, object> ToDictionary
            (this ApiGeoTaskCreateCommand from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { "AssistentActorsIds",
                    string.Join(',', from.AssistentActorsIds)},
                { "Description",  from.Description },
                { "GeosIds",  string.Join(',', from.GeosIds) },
                { "ObserverActorsIds",
                    string.Join(',', from.ObserverActorsIds) },
                { "PlanFinishAt",  from.PlanFinishAt },
                { "PlanStartAt",  from.PlanStartAt },
                { "ProjectId",  from.ProjectId },
                { "ResponsibleActorId",  from.ResponsibleActorId },
                { "Title",  from.Title },
            };
        }
    }
}
