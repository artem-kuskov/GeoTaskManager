using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using System.Linq;
using ApiEntityType = GeoTaskManager.Api.Projects.Models.ApiProject;
using EntityType = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Api.Projects.Mappers
{
    internal static class ProjectListResponseExtensions
    {
        public static ApiList<ApiEntityType> ToEntityList<TEntity>
            (this ListResponse<EntityType> from)
            where TEntity : ApiEntityType
        {
            if (from is null)
            {
                return null;
            }

            var result = new ApiList<ApiEntityType>
            {
                TotalCount = from.TotalCount
            };
            result.Entities.AddRange(from.Entities
                .Select(x => x.ToEntity<ApiEntityType>()));
            return result;
        }
    }
}
