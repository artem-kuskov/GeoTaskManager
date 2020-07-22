using System.Collections.Generic;

namespace GeoTaskManager.Api.Core.Models
{
    /// <summary>
    /// Response for the list of TEntity entities
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities</typeparam>
    public class ApiList<TEntity> where TEntity : class
    {
        /// <summary>
        /// List of the entities
        /// </summary>
        public List<TEntity> Entities { get; } = new List<TEntity>();

        /// <summary>
        /// Total count of requested entities when pagination is not applied.
        /// For example: if repository has 100 of requested entities,
        /// but response limited by 20 entities, TotalCount will be 100.
        /// </summary>
        public int TotalCount { get; set; }
    }
}
