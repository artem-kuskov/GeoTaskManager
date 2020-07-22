using GeoTaskManager.Application.Core.DbQueries;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Core.Mappers
{
    internal static class DbListRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary<TEntity>
            (this DbListRequest<TEntity> from) where TEntity : class
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                {nameof(from.Ids), String.Join(',', from.Ids) }
            };
        }
    }
}
