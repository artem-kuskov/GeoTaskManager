namespace GeoTaskManager.Api.Actors.Models
{
    /// <summary>
    /// Request for the list of Actor entities according applying filters.
    /// </summary>
    public class ApiActorListQuery
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
        /// Limit by Actor global role:
        ///     0 - any role,
        ///     1 - Admin,
        ///     2 - Manager,
        ///     4 - Actor,
        ///     8 - Observer,
        ///     or sum of several possible roles.
        ///     Example: 6 for Manager or Actor role
        /// </summary>
        public int ActorRoleMask { get; set; }

        /// <summary>
        /// Returns entities containing one 
        /// or several words from the parameter in the Title, Description, 
        /// FirstName, LastName, Department, Login
        /// </summary>
        public string ContainsKeyWords { get; set; }
    }
}
