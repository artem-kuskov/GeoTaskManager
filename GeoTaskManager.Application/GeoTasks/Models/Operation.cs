using Microsoft.AspNetCore.JsonPatch.Operations;

namespace GeoTaskManager.Application.GeoTasks.Models
{
    public class Operation
    {
        public OperationType OperationType { get; set; }
        public string Path { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
