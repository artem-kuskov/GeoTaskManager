using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.Application.Core.DbCommands
{
    public class DbCreateCommand<TEntity> : IRequest<CreateResult>
    {
        public TEntity Entity { get; set; }

        public DbCreateCommand()
        {

        }

        public DbCreateCommand(TEntity entity)
        {
            Entity = entity;
        }
    }
}
