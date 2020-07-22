using System.Collections.Generic;

namespace GeoTaskManager.Api.Core.Helpers
{
    public static class CollectionExtensions
    {
        public static void Add<TEntity>
            (this ICollection<TEntity> @this, IEnumerable<TEntity> entities)
        {
            if (entities != null && @this != null)
            {
                foreach (var item in entities)
                {
                    @this.Add(item);
                }
            }
        }
    }
}
