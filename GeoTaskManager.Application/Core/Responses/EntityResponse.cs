using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Responses
{
    public class EntityResponse<TEntity> where TEntity : class
    {
        public TEntity Entity { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public EntityResponse()
        {

        }

        public EntityResponse(TEntity entity)
        {
            Success = true;
            Entity = entity;
        }

        public EntityResponse(IEnumerable<string> errors)
        {
            Success = false;
            Errors.AddRange(errors);
        }
    }
}
