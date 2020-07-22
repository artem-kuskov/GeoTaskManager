using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.Application.Actors.DbQueries
{
    public class DbGetActorByNameRequest : IRequest<EntityResponse<Actor>>
    {
        public string UserName { get; set; }

        public DbGetActorByNameRequest()
        {

        }

        public DbGetActorByNameRequest(string userName)
        {
            UserName = userName;
        }
    }
}
