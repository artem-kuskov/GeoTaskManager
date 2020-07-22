using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.Application.Core.DbCommands
{
    public class DbUpdateCommand<TEntity> : IRequest<UpdateResult>
    {
        public TEntity Entity { get; set; }

        public DbUpdateCommand()
        {

        }

        public DbUpdateCommand(TEntity entity)
        {
            Entity = entity;
        }
    }
}
