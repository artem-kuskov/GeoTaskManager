using GeoTaskManager.Api.Actors.Models;
using System;

namespace GeoTaskManager.Api.Geos.Models
{
    /// <summary>
    /// Geo API model
    /// </summary>
    public class ApiGeo
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        public string Id { get; set; }

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
        public ApiActor CreatedBy { get; set; }

        /// <summary>
        /// Id of the Project, the Geo is linked to
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Serialized to string GeoJSON object
        /// (https://tools.ietf.org/html/rfc7946), 
        /// showing the position or area of Geo object
        /// </summary>
        public string GeoJson { get; set; }
    }
}
