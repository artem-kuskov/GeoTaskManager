using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.GeoTasks.Commands;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class ApiTaskUpdateModelExtensions
    {
        public static GeoTaskUpdateCommand ToGeoTaskUpdateCommand
            (this ApiGeoTaskUpdateCommand from, string id,
            ClaimsPrincipal currentPrincipal)
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
                ResponsibleActorId = from.ResponsibleActorId,
                Title = from.Title,
                CurrentPrincipal = currentPrincipal,
                IsArchived = from.IsArchived,
                Status = EnumerationClass
                    .GetAll<GeoTaskStatus>()
                    .Where(x => x.Id == from.Status)
                    .FirstOrDefault(),
                MessageTitle = from.MessageTitle,
                MessageDescription = from.MessageDescriprion
            };
            to.AssistentActorsIds.AddRange(from.AssistentActorsIds);
            to.GeosIds.AddRange(from.GeosIds);
            to.ObserverActorsIds.AddRange(from.ObserverActorsIds);
            return to;
        }

        public static Dictionary<string, object> ToDictionary
            (this ApiGeoTaskUpdateCommand from)
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
                { "IsArchived",  from.IsArchived },
                { "Status",  from.Status },
                { "MessageTitle", from.MessageTitle },
                { "MessageDescription", from.MessageDescriprion }
            };
        }
    }
}
