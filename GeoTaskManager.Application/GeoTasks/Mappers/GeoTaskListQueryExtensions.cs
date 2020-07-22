using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.GeoTasks.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Mappers
{
    public static class GeoTaskListQueryExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this GeoTaskListQuery from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.ActorId), from.ActorId },
                { nameof(from.ActorRoleMask), from.ActorRoleMask },
                { nameof(from.Archived), from.Archived },
                {
                    nameof(from.CurrentPrincipal),
                    from.CurrentPrincipal
                        .ToDictionary()
                        .ToList()
                        .Select(x => $"\"{x.Key}\"={x.Value}")
                },
                { nameof(from.Limit), from.Limit },
                { nameof(from.MaxTimeToDeadLine),
                    from.MaxTimeToDeadLine?.ToString() },
                { nameof(from.Offset),  from.Offset },
                { nameof(from.ProjectId), from.ProjectId },
                { nameof(from.TaskStatusMask),  from.TaskStatusMask },
                { nameof(from.СontainsKeyWords), from.СontainsKeyWords },
                { nameof(from.GeoIds), String.Join(',', from.GeoIds) },
            };
        }
    }
}
