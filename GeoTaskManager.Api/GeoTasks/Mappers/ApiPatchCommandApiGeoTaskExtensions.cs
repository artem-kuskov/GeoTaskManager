using GeoTaskManager.Api.Core.Commands;
using GeoTaskManager.Api.GeoTasks.Models;
using System.Collections.Generic;
using System.Text;

namespace GeoTaskManager.Api.GeoTasks.Mappers
{
    internal static class ApiPatchCommandApiGeoTaskExtensions
    {
        public static Dictionary<string, object> ToDictionary(this ApiPatchCommand<ApiGeoTask> from)
        {
            if (from is null)
            {
                return null;
            }

            var strBuilder = new StringBuilder();

            if (from.Patch != null)
            {
                foreach (var op in from.Patch.Operations)
                {
                    strBuilder.AppendLine($"op={op.op}, path={op.path}, value={op.value}");
                }
            }

            return new Dictionary<string, object>
            {
                { "Patch",  strBuilder.ToString()},
                { "MessageTitle", from.MessageTitle},
                { "MessageDescription", from.MessageDescription},
            };
        }
    }
}
