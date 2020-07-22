using GeoTaskManager.Application.Core.DbCommands;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Core.Mappers
{
    internal static class DbDeleteCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary<TEntity>
            (this DbDeleteCommand<TEntity> from) where TEntity : class
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
