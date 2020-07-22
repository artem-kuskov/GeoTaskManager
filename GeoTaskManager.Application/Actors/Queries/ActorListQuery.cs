using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Security.Claims;

namespace GeoTaskManager.Application.Actors.Queries
{
    public class ActorListQuery : IRequest<ListResponse<Actor>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public int ActorRoleMask { get; set; }
        public string СontainsKeyWords { get; set; }
        public ClaimsPrincipal CurrentPrincipal { get; set; }
    }
}
