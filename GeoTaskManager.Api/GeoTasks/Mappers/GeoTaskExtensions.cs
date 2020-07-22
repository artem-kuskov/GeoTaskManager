using GeoTaskManager.Api.Actors.Mappers;
using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Linq;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class GeoTaskExtensions
    {
        public static ApiGeoTask ToApiGeoTask(this GeoTask from)
        {
            if (from is null)
            {
                return null;
            }

            var to = new ApiGeoTask
            {
                Description = from.Description,
                Id = from.Id,
                PlanFinishAt = from.PlanFinishAt,
                PlanStartAt = from.PlanStartAt,
                ProjectId = from.ProjectId,
                ResponsibleActor = from.ResponsibleActor.ToApiActor(),
                Title = from.Title,
                CreatedAt = from.CreatedAt,
                CreatedBy = from.CreatedBy.ToApiActor(),
                IsArchived = from.IsArchived,
                Status = from.Status,
                StatusChangedAt = from.StatusChangedAt
            };
            to.AssistentActors.AddRange(from.AssistentActors.Select(x => x.ToApiActor()));
            to.GeosIds.AddRange(from.GeosIds);
            to.History.AddRange(from.History.Select(x => x.ToApiGeoTaskHistory()));
            to.ObserverActors.AddRange(from.ObserverActors.Select(x => x.ToApiActor()));

            return to;
        }
    }
}
