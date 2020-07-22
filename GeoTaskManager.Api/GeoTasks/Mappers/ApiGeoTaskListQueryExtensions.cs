using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Queries;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class ApiGeoTaskListQueryExtensions
    {
        public static GeoTaskListQuery ToGeoTaskListQuery
            (this ApiGeoTaskListQuery from, ClaimsPrincipal currentPrincipal)
        {
            if (from is null)
            {
                return null;
            }

            var result = new GeoTaskListQuery
            {
                ActorId = from.ActorId,
                ActorRoleMask = from.ActorRoleMask,
                Archived = from.Archived,
                CurrentPrincipal = currentPrincipal,
                Limit = from.Limit,
                Offset = from.Offset,
                ProjectId = from.ProjectId,
                TaskStatusMask = from.TaskStatusMask,
                СontainsKeyWords = from.ContainsKeyWords
            };

            if (TimeSpan.TryParse(from.MaxTimeToDeadLine,
                out var deadlineTimeSpan))
            {
                result.MaxTimeToDeadLine = deadlineTimeSpan;
            }

            return result;
        }

        public static Dictionary<string, object> ToDictionary
            (this ApiGeoTaskListQuery from)
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
                { nameof(from.Limit), from.Limit },
                { nameof(from.MaxTimeToDeadLine), from.MaxTimeToDeadLine },
                { nameof(from.Offset), from.Offset },
                { nameof(from.ProjectId), from.ProjectId },
                { nameof(from.TaskStatusMask), from.TaskStatusMask },
                { nameof(from.ContainsKeyWords), from.ContainsKeyWords },
                { nameof(from.GeoId), from.GeoId }
            };
        }
    }
}
