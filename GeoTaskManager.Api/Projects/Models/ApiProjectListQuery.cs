namespace GeoTaskManager.Api.Projects.Models
{
    /// <summary>
    /// Request for the list of Project entities according applying filters.
    /// </summary>
    public class ApiProjectListQuery
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
        /// Filter to include soft deleted entities:
        ///     'true' - return only soft deleted,
        ///     'false' - return only not soft deleted,
        ///      null - return any entities.
        /// </summary>
        public bool? Archived { get; set; }

        /// <summary>
        /// Text filter for entities containing one or several words
        /// in the Title or/and Description of Project.
        /// </summary>
        public string ContainsKeyWords { get; set; }
    }
}
