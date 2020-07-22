using GeoTaskManager.Application.GeoTasks.DbQueries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.MongoDb.GeoTasks.Mappers
{
    internal static class DbGetGeoTaskListRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary
             (this DbGetGeoTaskListRequest from)
        {
            return new Dictionary<string, object>
            {
                { nameof(from.Archived), from.Archived },
                { nameof(from.Contains), from.Contains },
                { nameof(from.Limit), from.Limit },
                { nameof(from.MaxTimeToDeadLine), from.MaxTimeToDeadLine },
                { nameof(from.Offset), from.Offset },
                { nameof(from.TaskStatusMask), from.TaskStatusMask },
                { nameof(from.Actors), String.Join(',',
                    from.Actors.Select(x => $"{x.Key}: {x.Value}")) },
                { nameof(from.ProjectIds), String.Join(',', from.ProjectIds) },
                { nameof(from.GeoIds), String.Join(',', from.GeoIds) },
            };
        }
    }
}
