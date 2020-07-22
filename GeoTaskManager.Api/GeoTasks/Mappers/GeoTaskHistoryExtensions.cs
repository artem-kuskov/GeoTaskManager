using GeoTaskManager.Api.Actors.Mappers;
using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.GeoTasks.Models;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class GeoTaskHistoryExtensions
    {
        public static ApiGeoTaskHistory ToApiGeoTaskHistory
            (this GeoTaskHistory from)
        {
            if (from is null)
            {
                return null;
            }

            return new ApiGeoTaskHistory
            {
                Description = from.Description,
                Id = from.Id,
                Title = from.Title,
                CreatedAt = from.ChangedAt,
                CreatedBy = from.ChangedBy.ToApiActor(),
            };
        }
    }
}
