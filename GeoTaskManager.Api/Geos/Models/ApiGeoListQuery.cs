namespace GeoTaskManager.Api.Geos.Models
{
    /// <summary>
    /// Request for the list of Project entities according applying filters.
    /// </summary>
    public class ApiGeoListQuery
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
        /// Filter Geos by specific Project they linked to
        /// {id} - Id of the Project,
        /// null - filter is not applied
        /// </summary>
        public string ProjectId { get; set; }

        /// DistanceLong (double) - set maximum distance (in meters) 
        /// to the entities from the CenterLong along Longitude coordinate 
        /// (a half of wide of the limit box with the center 
        /// in CenterLong, CenterLat), 
        /// {number} - distance in meters, 
        /// 0 - Geo spatial filter is not applied
        public double DistanceLong { get; set; }

        /// DistanceLat (double) - set maximum distance (in meters) 
        /// to the entities from the CenterLat along Latitude coordinate 
        /// (a half of height of the limit box with the center 
        /// in CenterLong, CenterLat), 
        /// {number} - distance in meters, 
        /// 0 - Geo spatial filter is not applied
        public double DistanceLat { get; set; }

        /// CenterLong (double) - longitude of the limit box center coordinate,
        /// from which DistanceLong is counted (values from -180 to 180).
        public double CenterLong { get; set; }

        /// CenterLat (double) - latitude of the limit box center coordinate,
        /// from which DistanceLat is counted (values from -90 to 90).
        public double CenterLat { get; set; }

        /// <summary>
        /// Text filter for entities containing one or several words
        /// in the Title or/and Description of Project.
        /// </summary>
        public string ContainsKeyWords { get; set; }
    }
}
