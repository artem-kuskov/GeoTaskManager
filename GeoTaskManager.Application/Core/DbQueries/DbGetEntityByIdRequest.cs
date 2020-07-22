using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.Application.Core.DbQueries
{
    public class DbGetEntityByIdRequest<TEntity>
        : IRequest<EntityResponse<TEntity>> where TEntity : class
    {
        public string Id { get; set; }

        public DbGetEntityByIdRequest()
        {

        }

        public DbGetEntityByIdRequest(string id)
        {
            Id = id;
        }
    }
}
