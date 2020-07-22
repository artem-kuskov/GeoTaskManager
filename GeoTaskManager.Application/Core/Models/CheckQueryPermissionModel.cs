using GeoTaskManager.Application.Actors.Models;
using System.Security.Claims;

namespace GeoTaskManager.Application.Core.Models
{
    public class CheckQueryPermissionModel<TEntity> where TEntity : class
    {
        public TEntity Entity { get; set; }
        public Actor Actor { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public ActorRole ProjectActorRole { get; set; }
    }
}
