namespace GeoTaskManager.Application.Configuration
{
    internal static class Defaults
    {
        public static string ConfigurationApiMaxLimitParameterName { get; } = "ApiConstraints:MaxEntityCollectionSize";
        public static int ConfigurationApiMaxLimitDefaultValue { get; } = 100;
        public static string ConfigurationApiDefaultLimitParameterName { get; } = "ApiConstraints:DefaultEntityCollectionSize";
        public static int ConfigurationApiDefaultLimitDefaultValue { get; } = 20;
        public static string ConfigurationActorsCreateAnonimousParameterName { get; } = "Actors:CreateAnonimousActor";
        public static bool ConfigurationActorsCreateAnonimousDefaultValue { get; } = false;
        public static char IdsSplitter { get; } = ',';

    }
}