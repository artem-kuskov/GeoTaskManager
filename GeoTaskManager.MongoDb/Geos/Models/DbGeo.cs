using GeoTaskManager.Application.Actors.Models;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace GeoTaskManager.MongoDb.Geos.Models
{
    internal class DbGeo
    {
        public string Id { get; set; }
        public bool IsArchived { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Actor CreatedBy { get; set; }
        public GeoJsonFeatureCollection<GeoJson2DGeographicCoordinates> GeoJson
        { get; set; }
        public string ProjectId { get; set; }
    }
}
