using System;

namespace GeoTaskManager.Application.Core.Models
{
    public static class EnumerationExtensions
    {
        public static bool Contains(this EnumerationClass value,
            EnumerationClass enumeration)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (enumeration is null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }

            return (value.Id & enumeration.Id) > 0;
        }
    }
}
