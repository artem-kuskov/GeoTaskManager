using System;
using System.Collections.Generic;

namespace GeoTaskManager.Application.Actors.Models
{
    public class Actor : IEquatable<Actor>
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public bool IsArchived { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string Skype { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public ActorRole Role { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Actor);
        }

        public bool Equals(Actor other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Actor left, Actor right)
        {
            return EqualityComparer<Actor>.Default.Equals(left, right);
        }

        public static bool operator !=(Actor left, Actor right)
        {
            return !(left == right);
        }
    }
}