using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.MongoDb.GeoTasks.Mappers
{
    internal static class DbGeoTaskExtensions
    {
        public static GeoTask ToGeoTask(this DbGeoTask from,
            Dictionary<string, Actor> actors)
        {
            actors.TryGetValue(from.CreatedById, out var creator);
            actors.TryGetValue(from.ResponsibleActorId, out var responsible);
            var assistents = from.AssistentActorsIds
                .Select(x =>
                    {
                        actors.TryGetValue(x, out var actor);
                        return actor;
                    })
                .Where(x => x != null)
                .ToList();
            var observers = from.ObserverActorsIds
                .Select(x =>
                {
                    actors.TryGetValue(x, out var actor);
                    return actor;
                })
                .Where(x => x != null)
                .ToList();

            var to = new GeoTask
            {
                CreatedAt = from.CreatedAt,
                Description = from.Description,
                Id = from.Id,
                IsArchived = from.IsArchived,
                PlanFinishAt = from.PlanFinishAt,
                PlanStartAt = from.PlanStartAt,
                ProjectId = from.ProjectId,
                Status = from.Status,
                StatusChangedAt = from.StatusChangedAt,
                Title = from.Title,
                CreatedBy = creator,
                ResponsibleActor = responsible,
            };
            to.AssistentActors.AddRange(assistents);
            to.ObserverActors.AddRange(observers);
            to.History.AddRange(from.History);
            to.GeosIds.AddRange(from.GeosIds);
            return to;
        }
    }
}
