using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.MongoDb.GeoTasks.Mappers
{
    internal static class DbCreateCommandGeoTaskExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbCreateCommand<GeoTask> from)
        {
            if (from is null)
            {
                return null;
            }

            if (from.Entity is null)
            {
                return new Dictionary<string, object>();
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Entity.AssistentActors),
                    string.Join(',', from.Entity.AssistentActors
                        .Select(x => x.Id)) },
                { nameof(from.Entity.CreatedAt), from.Entity.CreatedAt },
                { nameof(from.Entity.CreatedBy), from.Entity.CreatedBy?.Id },
                { nameof(from.Entity.Description), from.Entity.Description },
                { nameof(from.Entity.GeosIds),
                    string.Join(',', from.Entity.GeosIds) },
                { nameof(from.Entity.Id), from.Entity.Id },
                { nameof(from.Entity.IsArchived), from.Entity.IsArchived },
                { nameof(from.Entity.ObserverActors),
                    string.Join(',', from.Entity.ObserverActors
                        .Select(x => x.Id)) },
                { nameof(from.Entity.PlanFinishAt), from.Entity.PlanFinishAt },
                { nameof(from.Entity.PlanStartAt), from.Entity.PlanStartAt },
                { nameof(from.Entity.ProjectId), from.Entity.ProjectId },
                { nameof(from.Entity.ResponsibleActor),
                    from.Entity.ResponsibleActor?.Id },
                { nameof(from.Entity.Status), from.Entity.Status },
                { nameof(from.Entity.StatusChangedAt),
                    from.Entity.StatusChangedAt },
                { nameof(from.Entity.Title), from.Entity.Title },
            };
        }
    }
}
