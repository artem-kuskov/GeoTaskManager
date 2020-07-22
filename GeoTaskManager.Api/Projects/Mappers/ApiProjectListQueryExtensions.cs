using System.Collections.Generic;
using System.Security.Claims;
using _ApiEntityListQueryType =
    GeoTaskManager.Api.Projects.Models.ApiProjectListQuery;
using _EntityListQueryType =
    GeoTaskManager.Application.Projects.Queries.ProjectListQuery;
using _EntityType = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Api.Projects.Mappers
{
    internal static class ApiProjectListQueryExtensions
    {
        public static _EntityListQueryType ToListQuery<TEntity>
            (this _ApiEntityListQueryType from,
            ClaimsPrincipal currentPrincipal)
            where TEntity : _EntityType
        {
            if (from is null)
            {
                return null;
            }

            var result = new _EntityListQueryType
            {
                Archived = from.Archived,
                CurrentPrincipal = currentPrincipal,
                Limit = from.Limit,
                Offset = from.Offset,
                СontainsKeyWords = from.ContainsKeyWords
            };

            return result;
        }

        public static Dictionary<string, object> ToDictionary
            (this _ApiEntityListQueryType from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Archived),  from.Archived },
                { nameof(from.Limit),  from.Limit },
                { nameof(from.Offset),  from.Offset },
                { nameof(from.ContainsKeyWords),  from.ContainsKeyWords }
            };
        }
    }
}
