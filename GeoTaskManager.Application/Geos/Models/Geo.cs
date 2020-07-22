using GeoTaskManager.Application.Actors.Models;
using System;
using System.Collections.Generic;

namespace GeoTaskManager.Application.Geos.Models
{
    public class Geo : IEquatable<Geo>
    {
        public string Id { get; set; }
        public bool IsArchived { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Actor CreatedBy { get; set; }
        public string GeoJson { get; set; }
        public string ProjectId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Geo);
        }

        public bool Equals(Geo other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Geo left, Geo right)
        {
            return EqualityComparer<Geo>.Default.Equals(left, right);
        }

        public static bool operator !=(Geo left, Geo right)
        {
            return !(left == right);
        }
    }
}