using GeoTaskManager.Application.Projects.Models;
using System.Collections.Generic;

//Type aliases
using _EntityIdType = System.String;

namespace GeoTaskManager.Api.Projects.Models
{
    /// <summary>
    /// API command to create Project entity.
    /// </summary>
    public class ApiProjectCreateCommand
    {
        /// <summary>
        /// Soft delete attribute.
        ///     true - archived entity,
        ///     false - not archived entity.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Project roles for actors. 
        /// They are used to level up actor's global role
        /// in a specific project.
        /// It is a dictionary where the key is the Id of the actor 
        /// and the value is the Id of the role.
        /// There are the next roles:
        /// 1: Admin;
        /// 2: Manager;
        /// 4: Actor;
        /// 8: Observer.
        /// </summary>
        public Dictionary<_EntityIdType, int> ProjectActorRoles
        { get; private set; } = new Dictionary<_EntityIdType, int>();

        /// <summary>
        /// Indicate that the project's tasks are linked to geographic map
        /// </summary>
        public bool IsMap { get; set; }

        /// <summary>
        /// Name of map provider. It is used by front-end to show the map.
        /// </summary>
        public string MapProvider { get; set; }

        /// <summary>
        /// Dictionary of the map visualization parameters. 
        /// It is used by front-end to show the map.
        /// </summary>
        public Dictionary<string, object> MapParameters
        { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// Show or hide map layer at the front-end.
        /// </summary>
        public bool ShowMap { get; set; }

        /// <summary>
        /// Overlay image layers showed as background for the project's tasks.
        /// </summary>
        public List<VisualLayer> Layers
        { get; private set; } = new List<VisualLayer>();
    }
}
