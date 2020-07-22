using GeoTaskManager.Api.Actors.Models;
using System;

namespace GeoTaskManager.Api.GeoTasks.Models
{
    /// <summary>
    /// GeoTask history of changes API model.
    /// </summary>
    public class ApiGeoTaskHistory
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The title of the change commit.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the change commit.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date and time of change commit in UTC format.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Actor performed the change.
        /// </summary>
        public ApiActor CreatedBy { get; set; }
    }
}