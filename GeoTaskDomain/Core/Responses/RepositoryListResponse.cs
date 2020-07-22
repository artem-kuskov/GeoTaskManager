using System.Collections.Generic;

namespace GeoTaskManager.Domain.Core.Responses
{
    public class RepositoryListResponse<TEntity> where TEntity : class
    {
        public List<TEntity> Entities { get; } = new List<TEntity>();
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

    }
}
