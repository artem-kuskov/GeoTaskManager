using System.Collections.Generic;

namespace GeoTaskManager.Domain.Core.Responses
{
    public class RepositoryResponse<TEntity> where TEntity : class
    {
        public TEntity Entity { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public RepositoryResponse()
        {

        }

        public RepositoryResponse(TEntity entity, bool success, IList<string> errors)
        {
            Entity = entity;
            Success = success;
            Errors.AddRange(errors);
        }
    }
}
