using System.Collections.Generic;

namespace GeoTaskManager.Api.Core.Filters
{
    internal class ApiErrorResponse
    {
        public List<ApiErrorModel> Errors { get; } = new List<ApiErrorModel>();
    }
}
