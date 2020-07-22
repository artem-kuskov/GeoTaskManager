using System;

namespace GeoTaskManager.Application.Core.Models
{
    public static class IntExtensions
    {
        public static bool Contains(this int value,
            EnumerationClass enumeration)
        {
            if (enumeration is null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }

            return (value & enumeration.Id) > 0;
        }
    }
}
