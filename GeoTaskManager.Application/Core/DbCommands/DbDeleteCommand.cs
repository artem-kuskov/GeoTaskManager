using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.Application.Core.DbCommands
{
    public class DbDeleteCommand<TEntity> : IRequest<DeleteResult>
    {
        public string Id { get; set; }

        public DbDeleteCommand()
        {

        }

        public DbDeleteCommand(string id)
        {
            Id = id;
        }
    }
}
