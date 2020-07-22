using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.Actors.Models
{
    public class ActorRole : EnumerationClass
    {
        public static readonly ActorRole Admin = new ActorRole(1, "Admin");
        public static readonly ActorRole Manager = new ActorRole(2, "Manager");
        public static readonly ActorRole Actor = new ActorRole(4, "Actor");
        public static readonly ActorRole Observer = new ActorRole(8, "Observer");

        public ActorRole(int id, string name) : base(id, name)
        {

        }
    }
}