using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Application.GeoTasks.Models;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.Api.GeoTasks.Models
{
    /// <summary>
    /// GeoTask API model
    /// </summary>
    public class ApiGeoTask
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
        /// GeoTask title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date and time of GeoTask creation in UTC format.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Plan date and time of GeoTask start in UTC format.
        /// </summary>
        public DateTime? PlanStartAt { get; set; }

        /// <summary>
        /// Plan date and time of GeoTask finish in UTC format.
        /// </summary>
        public DateTime? PlanFinishAt { get; set; }

        /// <summary>
        /// GeoTask status.
        /// </summary>
        public GeoTaskStatus Status { get; set; }

        /// <summary>
        /// Date and time of last status change in UTC format.
        /// </summary>
        public DateTime StatusChangedAt { get; set; }

        /// <summary>
        /// Id of the Project, GeoTask connected to.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Actor, the GeoTask created by.
        /// </summary>
        public ApiActor CreatedBy { get; set; }

        /// <summary>
        /// Actor responsible for GeoTask's result.
        /// </summary>
        public ApiActor ResponsibleActor { get; set; }

        /// <summary>
        /// Assistant actors
        /// </summary>
        public List<ApiActor> AssistentActors { get; } = new List<ApiActor>();

        /// <summary>
        /// Actors those observe the process of GeoTask execution 
        /// but do not participate in it.
        /// </summary>
        public List<ApiActor> ObserverActors { get; } = new List<ApiActor>();

        /// <summary>
        /// Ids of Geo entities connected to GeoTask.
        /// </summary>
        public List<string> GeosIds { get; } = new List<string>();

        /// <summary>
        /// The history of GeoTask changes.
        /// </summary>
        public List<ApiGeoTaskHistory> History { get; } = new List<ApiGeoTaskHistory>();
    }
}
