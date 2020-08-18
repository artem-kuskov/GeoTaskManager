using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using System.Linq;

namespace GeoTaskManager.MongoDb.GeoTasks.Mappers
{
    internal static class GeoTaskExtensions
    {
        public static DbGeoTask ToDbGeoTask(this GeoTask from)
        {
            if (from is null)
            {
                return null;
            }

            var to = new DbGeoTask
            {
                CreatedAt = from.CreatedAt,
                Description = from.Description,
                CreatedById = from.CreatedBy?.Id,
                Id = from.Id,
                IsArchived = from.IsArchived,
                PlanFinishAt = from.PlanFinishAt,
                PlanStartAt = from.PlanStartAt,
                ProjectId = from.ProjectId,
                Status = from.Status,
                ResponsibleActorId = from.ResponsibleActor?.Id,
                StatusChangedAt = from.StatusChangedAt,
                Title = from.Title
            };
            to.AssistentActorsIds.AddRange
                (from.AssistentActors.Select(x => x.Id));
            to.GeosIds.AddRange(from.GeosIds);
            to.History.AddRange(from.History);
            to.ObserverActorsIds.AddRange
                (from.ObserverActors.Select(x => x.Id));

            return to;
        }
    }
}