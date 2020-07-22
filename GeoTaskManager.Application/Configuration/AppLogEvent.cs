using Microsoft.Extensions.Logging;

namespace GeoTaskManager.Application.Configuration
{
    internal static class AppLogEvent
    {
        public static EventId HandleRequest { get; } = new EventId(10_10_01, "Handle request or command");
        public static EventId RequestValidation { get; } = new EventId(10_11_01, "Validation of the request or command");
        public static EventId SecurityValidation { get; } = new EventId(10_11_02, "Security validation of the request or command");
        public static EventId HandleInit { get; } = new EventId(10_10_10, "Handle repository initialization");

        public static EventId HandleArgumentError { get; } = new EventId(20_10_10, "Argument error in handle request or command");
        public static EventId HandleNullResponse { get; } = new EventId(20_10_20, "Null response in handle");
        public static EventId RequestNotValid { get; } = new EventId(20_11_01, "Validation of the request or command not passed");
        public static EventId SecurityNotPassed { get; } = new EventId(20_11_02, "Security validation of the request or command not passed");

        public static EventId HandleErrorResponse { get; } = new EventId(30_10_30, "Error response in handle");
        public static EventId HandleInitError { get; } = new EventId(30_10_31, "Handle repository initialization error");
        public static EventId RequestValidationError { get; } = new EventId(30_11_01, "Error while validation of the request or command");
        public static EventId SecurityValidationError { get; } = new EventId(30_11_02, "Error while security validation of the request or command");
    }
}
