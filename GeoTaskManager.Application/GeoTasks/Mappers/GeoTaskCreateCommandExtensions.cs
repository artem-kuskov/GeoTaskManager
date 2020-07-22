using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.GeoTasks.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Mappers
{
    public static class GeoTaskCreateCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this GeoTaskCreateCommand from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Description), from.Description },
                { nameof(from.GeosIds), String.Join(',', from.GeosIds) },
                { nameof(from.IsArchived),  from.IsArchived },
                { nameof(from.PlanFinishAt), from.PlanFinishAt },
                { nameof(from.PlanStartAt), from.PlanStartAt },
                { nameof(from.ProjectId), from.ProjectId },
                { nameof(from.Title), from.Title },
                {
                    nameof(from.AssistentActorsIds),
                    String.Join(',', from.AssistentActorsIds)
                },
                {
                    nameof(from.CurrentPrincipal),
                    from.CurrentPrincipal
                        .ToDictionary()
                        .ToList()
                        .Select(x => $"\"{x.Key}\"={x.Value}")
                },
                { nameof(from.GeosIds), String.Join(',', from.GeosIds) },
                {
                    nameof(from.ObserverActorsIds),
                    String.Join(',', from.ObserverActorsIds)
                },
                { nameof(from.ResponsibleActorId), from.ResponsibleActorId },
            };
        }
    }
}
