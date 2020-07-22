using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.GeoTasks.DbQueries;

namespace GeoTaskManager.Application.GeoTasks.Security
{
    internal class DbGetGeoTaskListRequestSecurityBuilder
    {
        private DbGetGeoTaskListRequest Filter { get; set; }
        private bool Builded { get; set; } = false;
        private DbGetGeoTaskListRequest SecuredFilter { get; set; }
        private Actor Actor { get; set; }
        private ActorRole ProjectActorRole { get; set; }

        public DbGetGeoTaskListRequestSecurityBuilder
            (DbGetGeoTaskListRequest filter, Actor currentActor,
            ActorRole projectActorRole = null)
        {
            if (filter is null)
            {
                throw new System.ArgumentNullException(nameof(filter));
            }

            if (currentActor is null)
            {
                throw new System.ArgumentNullException(nameof(currentActor));
            }

            Filter = filter;
            Actor = currentActor;
            ProjectActorRole = projectActorRole;
        }

        public DbGetGeoTaskListRequest Build()
        {
            if (Builded)
            {
                return SecuredFilter;
            }

            SecuredFilter = Filter.Copy();

            // Global roles of Admin, Manager and Observer 
            // allow to get all tasks from all projects
            if (Actor.Role == ActorRole.Admin
                || Actor.Role == ActorRole.Manager
                || Actor.Role == ActorRole.Observer)
            {
                Builded = true;
                return SecuredFilter;
            }

            // Access of the global role Actor is allowed only to the tasks 
            // where the current actor participates or to the all tasks 
            // in projects where actor has Project Role of 
            // Admin, Manager or Observer.
            // 
            // If only one Project in request, 
            // then Project Role is used to check access. 
            //
            // If there is no Project Id or there are more that one Project Id, 
            // the Global Role is used
            if (Filter.ProjectIds.Count == 1)
            {
                if (ProjectActorRole != null
                    && (ProjectActorRole == ActorRole.Admin
                        || ProjectActorRole == ActorRole.Manager
                        || ProjectActorRole == ActorRole.Observer
                        ))
                {
                    Builded = true;
                    return SecuredFilter;
                }
            }

            // Ensure that filter contain current actor
            if (!SecuredFilter.Actors.ContainsKey(Actor.Id))
            {
                SecuredFilter.Actors[Actor.Id] = 0;
            }

            Builded = true;
            return SecuredFilter;
        }
    }
}