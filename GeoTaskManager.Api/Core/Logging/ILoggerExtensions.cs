using Microsoft.Extensions.Logging;
using System;

namespace GeoTaskManager.Api.Core.Logging
{
    internal static class ILoggerExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public static bool WriteScopeWhenException(this ILogger logger, EventId eventId, Exception exception)
        {
            logger.LogError(eventId, exception, "Unexpected error");
            return true;
        }
    }
}
