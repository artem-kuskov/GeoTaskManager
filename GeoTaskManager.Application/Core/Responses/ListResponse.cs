using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Responses
{
    public class ListResponse<TEntity> where TEntity : class
    {
        public List<TEntity> Entities { get; } = new List<TEntity>();
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public ListResponse()
        {

        }

        public ListResponse(IEnumerable<TEntity> entities, int totalCount)
        {
            Success = true;
            Entities.AddRange(entities);
            TotalCount = totalCount;
        }

        public ListResponse(IEnumerable<string> errors)
        {
            Success = false;
            Errors.AddRange(errors);
        }
    }
}
