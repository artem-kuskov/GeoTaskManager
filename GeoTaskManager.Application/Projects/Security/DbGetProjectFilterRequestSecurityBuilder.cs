using GeoTaskManager.Application.Actors.Models;

// Type alias
using _TFilter = GeoTaskManager.Application.Projects.DbQueries.DbGetProjectFilterRequest;

namespace GeoTaskManager.Application.Projects.Security
{
    internal class DbGetProjectFilterRequestSecurityBuilder
    {
        private _TFilter Filter { get; set; }
        private bool Builded { get; set; } = false;
        private _TFilter SecuredFilter { get; set; }
        private Actor Actor { get; set; }

        public DbGetProjectFilterRequestSecurityBuilder(_TFilter filter,
            Actor currentActor)
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
        }

        public _TFilter Build()
        {
            if (Builded)
            {
                return SecuredFilter;
            }

            SecuredFilter = Filter.Copy();

            // All current actors have access to get all projects
            Builded = true;
            return SecuredFilter;
        }
    }
}