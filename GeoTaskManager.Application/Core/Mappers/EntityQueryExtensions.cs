using GeoTaskManager.Application.Core.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.Core.Mappers
{
    public static class EntityQueryExtensions
    {
        public static Dictionary<string, object> ToDictionary<TEntity>
            (this EntityQuery<TEntity> from) where TEntity : class
        {
            if (from is null)
            {
                return null;
            }
            var result = new Dictionary<string, object>
            {
                {
                    nameof(from.Id),
                    from.Id
                },
                {
                    nameof(from.CurrentPrincipal),
                    String.Join(',', from.CurrentPrincipal.ToDictionary()
                        .Select(x => $"{x.Key}={x.Value}"))
                },
            };
            return result;
        }
    }
}
