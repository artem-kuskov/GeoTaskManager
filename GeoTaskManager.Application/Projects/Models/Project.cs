using GeoTaskManager.Application.Actors.Models;
using System;
using System.Collections.Generic;

//Type aliases
using _TEntityId = System.String;


namespace GeoTaskManager.Application.Projects.Models
{
    public class Project : IEquatable<Project>
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        public _TEntityId Id { get; set; }

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
        /// Date and time of entity creation in UTC format.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Actor, the entity created by.
        /// </summary>
        public Actor CreatedBy { get; set; }

        /// <summary>
        /// Project roles for actors. 
        /// They are used to level up actor's global role
        /// in a specific project.
        /// </summary>
        public Dictionary<_TEntityId, ActorRole> ProjectActorRoles
        { get; private set; } = new Dictionary<_TEntityId, ActorRole>();

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Project);
        }

        public bool Equals(Project other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Project left, Project right)
        {
            return EqualityComparer<Project>.Default.Equals(left, right);
        }

        public static bool operator !=(Project left, Project right)
        {
            return !(left == right);
        }
    }
}