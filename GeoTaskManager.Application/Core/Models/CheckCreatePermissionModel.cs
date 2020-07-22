using GeoTaskManager.Application.Actors.Models;

namespace GeoTaskManager.Application.Core.Models
{
    public class CheckCreatePermissionModel<TEntity> where TEntity : class
    {
        public TEntity Entity { get; set; }
        public Actor Actor { get; set; }
        public ActorRole ProjectActorRole { get; set; }
    }
}
