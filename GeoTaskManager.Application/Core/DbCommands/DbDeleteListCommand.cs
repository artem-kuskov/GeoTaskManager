using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.DbCommands
{
    public class DbDeleteListCommand<TEntity> : IRequest<DeleteResult>
    {
        public List<string> Ids { get; private set; } = new List<string>();

        public DbDeleteListCommand()
        {

        }

        public DbDeleteListCommand(List<string> ids)
        {
            Ids.AddRange(ids);
        }
    }
}
