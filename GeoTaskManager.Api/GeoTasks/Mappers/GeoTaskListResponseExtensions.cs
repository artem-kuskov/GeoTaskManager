using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Linq;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class GeoTaskListResponseExtensions
    {
        public static ApiList<ApiGeoTask> ToApiGeoTaskList
            (this ListResponse<GeoTask> from)
        {
            if (from is null)
            {
                return null;
            }

            var result = new ApiList<ApiGeoTask>
            {
                TotalCount = from.TotalCount
            };
            result.Entities.AddRange(from.Entities
                .Select(x => x.ToApiGeoTask()));
            return result;
        }
    }
}
