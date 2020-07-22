using Microsoft.Extensions.Logging;

namespace GeoTaskManager.MongoDb.Configuration
{
    internal static class LogEvent
    {
        public static EventId DatabaseRequest { get; }
            = new EventId(10_20_01, "Database request or command");
        public static EventId DatabaseInit { get; }
            = new EventId(10_20_10, "Database initialization");
        public static EventId DatabaseRequestArgumentError { get; }
            = new EventId(20_20_10, "Database argument error");
        public static EventId DatabaseEmptyResponse { get; }
            = new EventId(20_20_20, "Null or empty response from database " +
                "query or command");
        public static EventId DatabaseExceptionError { get; }
            = new EventId(30_20_30, "Database exception error");
        public static EventId DatabaseInitError { get; }
            = new EventId(30_20_31, "Database initialization error");
    }
}
