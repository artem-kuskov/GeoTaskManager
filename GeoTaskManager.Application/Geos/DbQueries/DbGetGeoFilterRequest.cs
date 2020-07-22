using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.Models;
using MediatR;

namespace GeoTaskManager.Application.Geos.DbQueries
{
    public class DbGetGeoFilterRequest : IRequest<ListResponse<Geo>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public string Contains { get; set; }

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
    }
}
