using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GeoTaskManager.Application.Core.Models
{
    public abstract class EnumerationClass : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected EnumerationClass(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : EnumerationClass
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            var values = fields.Select(f => f.GetValue(null)).Cast<T>();
            return values;
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as EnumerationClass;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public int CompareTo(object other)
        {
            if (!(other is EnumerationClass to))
            {
                throw new ArgumentException("Not comparable argument",
                    nameof(other));
            }

            return Id.CompareTo(to.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(EnumerationClass left,
            EnumerationClass right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(EnumerationClass left,
            EnumerationClass right)
        {
            return !(left == right);
        }

        public static bool operator <(EnumerationClass left,
            EnumerationClass right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(EnumerationClass left,
            EnumerationClass right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(EnumerationClass left,
            EnumerationClass right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(EnumerationClass left,
            EnumerationClass right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
