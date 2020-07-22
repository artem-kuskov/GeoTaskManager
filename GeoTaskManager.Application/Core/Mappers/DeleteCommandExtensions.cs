using GeoTaskManager.Application.Core.Commands;
using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Mappers
{
    public static class DeleteCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary<TEntity>
            (this DeleteCommand<TEntity> from) where TEntity : class
        {
            if (from is null)
            {
                return null;
            }
            var result = new Dictionary<string, object>
            {
                {
                    nameof(from.CurrentPrincipal),
                    from.CurrentPrincipal?.Identity?.Name
                },
                { nameof(from.HardMode), from.HardMode },
                { nameof(from.Id), from.Id },
                { nameof(from.MessageTitle), from.MessageTitle },
                { nameof(from.MessageDescription), from.MessageDescription }
            };
            return result;
        }
    }
}
