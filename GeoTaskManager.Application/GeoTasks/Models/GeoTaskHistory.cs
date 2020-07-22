using GeoTaskManager.Application.Actors.Models;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.Application.GeoTasks.Models
{
    public class GeoTaskHistory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ChangedAt { get; set; }
        public virtual Actor ChangedBy { get; set; }
        public List<Operation> Operations { get; private set; } =
            new List<Operation>();
    }
}