using GeoTaskManager.Application.Actors.Models;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.Application.GeoTasks.Models
{
    public class GeoTask : IEquatable<GeoTask>
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
        public Actor CreatedBy { get; set; }
        public Actor ResponsibleActor { get; set; }
        public List<Actor> AssistentActors { get; private set; } =
            new List<Actor>();
        public List<Actor> ObserverActors { get; private set; } =
            new List<Actor>();
        public List<string> GeosIds { get; private set; } =
            new List<string>();
        public List<GeoTaskHistory> History { get; private set; } =
            new List<GeoTaskHistory>();

        public override bool Equals(object obj)
        {
            return Equals(obj as GeoTask);
        }

        public bool Equals(GeoTask other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(GeoTask left, GeoTask right)
        {
            return EqualityComparer<GeoTask>.Default.Equals(left, right);
        }

        public static bool operator !=(GeoTask left, GeoTask right)
        {
            return !(left == right);
        }
    }
}
