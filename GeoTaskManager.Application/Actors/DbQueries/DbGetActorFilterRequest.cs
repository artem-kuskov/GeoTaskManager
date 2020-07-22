using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Responses;
using MediatR;

namespace GeoTaskManager.Application.Actors.DbQueries
{
    public class DbGetActorFilterRequest : IRequest<ListResponse<Actor>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public string Contains { get; set; }
        public int ActorRoleMask { get; set; }
    }
}
