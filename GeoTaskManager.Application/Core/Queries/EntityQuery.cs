using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Security.Claims;

namespace GeoTaskManager.Application.Core.Queries
{
    public class EntityQuery<TEntity> : IRequest<EntityResponse<TEntity>>
        where TEntity : class
    {
        public string Id { get; set; }
        public ClaimsPrincipal CurrentPrincipal { get; set; }

        public EntityQuery()
        {

        }

        public EntityQuery(string id, ClaimsPrincipal currentPrincipal)
        {
            Id = id;
            CurrentPrincipal = currentPrincipal;
        }
    }
}
