namespace GeoTaskManager.Api.GeoTasks.Models
{
    /// <summary>
    /// Request for the list of GeoTask entities according applying filters.
    /// </summary>
    public class ApiGeoTaskListQuery
    {
        /// <summary>
        /// Offset for pagination of the list.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Limit the list length for pagination.
        /// API has self limits and when Limit exceeds API limits
        /// the value will be decreased.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Filter to include archived entities:
        ///     'true' - return only archived,
        ///     'false' - return only not archived,
        ///      null - return any entities.
        /// </summary>
        public bool? Archived { get; set; }

        /// <summary>
        /// Filter to include only entities connected to specific Project:
        ///     Id of specific Project entity,
        ///     null for all entities.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Filter for specific Actor participated in GeoTask:
        ///     Id of the Actor,
        ///     null for any entities.
        /// </summary>
        public string ActorId { get; set; }

        /// <summary>
        /// Limit by ActorId's role in the GeoTask:
        ///     0 - any role,
        ///     1 - Creator,
        ///     2 - Responsible,
        ///     4 - Assistant,
        ///     8 - Observer,
        ///     or sum of several possible roles.
        ///     Example: 6 for responsible or assistant role
        /// </summary>
        public int ActorRoleMask { get; set; }

        /// <summary>
        /// Limit GeoTask status:
        ///     0 - any status;
        ///     1 - New;
        ///     2 - In Work;
        ///     4 - Finish requested;
        ///     8 - Finished;
        ///     16 - Cancel requested;
        ///     32 - Canceled;
        ///     or sum of several possible statuses.
        ///     Example: 48 for canceled or requested for cancellation 
        ///              Geo Tasks.
        /// </summary>
        public int TaskStatusMask { get; set; }

        /// <summary>
        /// Maximum time lag from current time in UTC format 
        /// to planned finish time in UTC format. It can be negative time.
        ///     Example: 
        ///         "6.20:20:10" returns Geo Tasks 
        ///             where the planned finish time was in the past 
        ///             or will be in the next 6 days 20 hours 20 minutes
        ///             and 10 seconds
        ///         "-6.20:20:10" returns Geo Tasks 
        ///         where the planned finish time was 6 days 20 hours 
        ///         20 minutes and 10 seconds ago
        /// </summary>
        public string MaxTimeToDeadLine { get; set; }

        /// <summary>
        /// Text filter for entities containing one or several words
        /// in the Title or/and Description of GeoTask.
        /// </summary>
        public string ContainsKeyWords { get; set; }

        /// <summary>
        /// The Id of Geospatial entity, the GeoTask is linked to.
        /// </summary>
        public string GeoId { get; set; }
    }
}
