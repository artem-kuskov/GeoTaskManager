using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.Actors.Models
{
    public class ActorGeoTaskRole : EnumerationClass
    {
        public static readonly ActorGeoTaskRole Creator = new ActorGeoTaskRole(1, "Creator");
        public static readonly ActorGeoTaskRole Responsible = new ActorGeoTaskRole(2, "Responsible");
        public static readonly ActorGeoTaskRole Assistant = new ActorGeoTaskRole(4, "Assistant");
        public static readonly ActorGeoTaskRole Observer = new ActorGeoTaskRole(8, "Observer");

        public ActorGeoTaskRole(int id, string name) : base(id, name)
        {
        }
    }
}