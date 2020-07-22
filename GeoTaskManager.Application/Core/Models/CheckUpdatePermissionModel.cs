using GeoTaskManager.Application.Actors.Models;

namespace GeoTaskManager.Application.Core.Models
{
    public class CheckUpdatePermissionModel<TEntity> where TEntity : class
    {
        public TEntity EntityBeforeUpdate { get; set; }
        public TEntity EntityAfterUpdate { get; set; }
        public Actor Actor { get; set; }
        public ActorRole OldProjectRole { get; set; }
        public ActorRole NewProjectRole { get; set; }
    }
}
