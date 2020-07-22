using System.Collections.Generic;

namespace GeoTaskManager.Domain.Core.Responses
{
    public class RepositoryDeleteResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }
}
