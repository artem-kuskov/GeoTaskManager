using GeoTaskManager.Application.Core.DbQueries;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Core.Mappers
{
    internal static class DbGetEntityByIdRequestExtensions
    {
        public static Dictionary<string, object> ToDictionary<TEntity>
            (this DbGetEntityByIdRequest<TEntity> from) where TEntity : class
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Id), from.Id }
            };
        }
    }
}
