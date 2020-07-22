using GeoTaskManager.Application.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using _TEntity =
    GeoTaskManager.Application.Projects.Queries.ProjectListQuery;

namespace GeoTaskManager.Application.Projects.Mappers
{
    public static class ProjectListQueryExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this _TEntity from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Archived), from.Archived },
                { nameof(from.Limit), from.Limit},
                { nameof(from.Offset), from.Offset },
                { nameof(from.СontainsKeyWords), from.СontainsKeyWords},
                { nameof(from.CurrentPrincipal),
                    String.Join(',',
                        from.CurrentPrincipal
                            .ToDictionary()
                            .Select(x => $"{x.Key}={x.Value}"))},
            };
        }
    }
}
