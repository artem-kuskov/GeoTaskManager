namespace GeoTaskManager.MongoDb.Configuration
{
    internal static class Defaults
    {
        public static string ConnectionStringParameterName { get; }
            = "MongoDbConnection";
        public static string ConnectionStringDefaultValue { get; }
            = "mongodb://localhost:27017/?appname=GeoTaskManagerApi&ssl=false";
        public static string DatabaseNameParameterName { get; }
            = "MongoDbName";
        public static string DatabaseNameDefaultValue { get; }
            = "geotaskmanager";
        public static string TaskCollectionName { get; } = "geotasks";
        public static string ProjectCollectionName { get; } = "projects";
        public static string ActorCollectionName { get; } = "actors";
        public static string GeoCollectionName { get; } = "geos";

    }
}
