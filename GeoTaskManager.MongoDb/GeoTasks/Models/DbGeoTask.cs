using GeoTaskManager.Application.GeoTasks.Models;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.GeoTasks.Models
{
    internal class DbGeoTask
    {
        public string Id { get; set; }
        public bool IsArchived { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PlanStartAt { get; set; }
        public DateTime? PlanFinishAt { get; set; }
        public GeoTaskStatus Status { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public string ProjectId { get; set; }
        public string CreatedById { get; set; }
        public string ResponsibleActorId { get; set; }
        public List<string> AssistentActorsIds { get; private set; }
            = new List<string>();
        public List<string> ObserverActorsIds { get; private set; }
            = new List<string>();
        public List<string> GeosIds { get; private set; } = new List<string>();
        public List<GeoTaskHistory> History { get; private set; }
            = new List<GeoTaskHistory>();
    }
}
