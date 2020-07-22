using System;
using System.Collections.Generic;

namespace GeoTaskManager.Api.GeoTasks.Models
{
    /// <summary>
    /// Update GeoTask entity command
    /// </summary>
    public class ApiGeoTaskUpdateCommand
    {
        /// <summary>
        /// The title of GeoTask
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Detail description of the GeoTask.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date and time in UTC format for planning start time.
        /// If 'null' then there is no planning date.
        /// </summary>
        public DateTime? PlanStartAt { get; set; }

        /// <summary>
        /// Date and time in UTC format for planning finish time.
        /// If 'null' then there is no planning date.
        /// </summary>
        public DateTime? PlanFinishAt { get; set; }

        /// <summary>
        /// Id of the Project entity that GeoTask connecting to.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Id of the Actor entity that has responsibility for GeoTask result.
        /// </summary>
        public string ResponsibleActorId { get; set; }

        /// <summary>
        /// List of Ids of the Actors those play assistant role 
        /// and help to responsible actor.
        /// </summary>
        public List<string> AssistentActorsIds { get; } = new List<string>();

        /// <summary>
        /// List of Ids of the Actors entities 
        /// those do not participate in GeoTask execution, 
        /// but can observe the process.
        /// </summary>
        public List<string> ObserverActorsIds { get; private set; }
            = new List<string>();

        /// <summary>
        /// List of Ids of the Geo entities where GeoTask must be executed.
        /// </summary>
        public List<string> GeosIds { get; private set; } = new List<string>();

        /// <summary>
        /// Soft delete attribute:
        ///     true - entity is archived,
        ///     false - entity is not archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// GeoTask status:
        ///     1 - New,
        ///     2 - In Work,
        ///     4 - Finish requested,
        ///     8 - Finished,
        ///     16 - Cancel requested,
        ///     32 - Canceled.
        public int Status { get; set; }

        /// <summary>
        /// Commit changes message title.
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// Commit changes message description.
        /// </summary>
        public string MessageDescriprion { get; set; }
    }
}
