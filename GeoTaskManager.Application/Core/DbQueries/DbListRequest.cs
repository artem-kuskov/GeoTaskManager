using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.DbQueries
{
    public class DbListRequest<TEntity>
        : IRequest<ListResponse<TEntity>> where TEntity : class
    {
        public IEnumerable<string> Ids { get; set; }

        public DbListRequest()
        {

        }

        public DbListRequest(IEnumerable<string> ids)
        {
            Ids = ids;
        }
    }
}
