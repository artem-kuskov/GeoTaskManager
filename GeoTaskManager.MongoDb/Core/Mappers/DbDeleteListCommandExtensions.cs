using GeoTaskManager.Application.Core.DbCommands;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Core.Mappers
{
    internal static class DbDeleteListCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary<TEntity>
            (this DbDeleteListCommand<TEntity> from) where TEntity : class
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Ids), String.Join(',', from.Ids) }
            };
        }
    }
}
