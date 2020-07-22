using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.GeoTasks.Models
{
    public class GeoTaskStatus : EnumerationClass
    {
        public static readonly GeoTaskStatus New =
            new GeoTaskStatus(1, "New");
        public static readonly GeoTaskStatus InWork =
            new GeoTaskStatus(2, "In Work");
        public static readonly GeoTaskStatus FinishRequested =
            new GeoTaskStatus(4, "Finish Requested");
        public static readonly GeoTaskStatus Finished =
            new GeoTaskStatus(8, "Finished");
        public static readonly GeoTaskStatus CancelRequested =
            new GeoTaskStatus(16, "Cancel Requested");
        public static readonly GeoTaskStatus Canceled =
            new GeoTaskStatus(32, "Canceled");

        public GeoTaskStatus(int id, string name) : base(id, name)
        {

        }
    }
}