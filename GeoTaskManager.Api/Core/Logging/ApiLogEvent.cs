using Microsoft.Extensions.Logging;

namespace GeoTaskManager.Api.Core.Logging
{
    internal static class ApiLogEvent
    {
        public static EventId ApiRequest { get; } = new EventId(10_01_01, "API request");
        public static EventId ApiArgumentError { get; } = new EventId(20_01_10, "API argument error");
        public static EventId ApiErrorResponse { get; } = new EventId(30_01_30, "API error response");

    }
}
