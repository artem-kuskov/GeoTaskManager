using System.Collections.Generic;

namespace GeoTaskManager.Domain.Core.Responses
{
    public class RepositoryUpdateResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }
}
