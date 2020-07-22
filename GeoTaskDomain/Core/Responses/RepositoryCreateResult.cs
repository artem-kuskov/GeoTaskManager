using System.Collections.Generic;

namespace GeoTaskManager.Domain.Core.Responses
{
    public class RepositoryCreateResult
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public RepositoryCreateResult()
        {

        }

        public RepositoryCreateResult(string id, bool success, ICollection<string> errors)
        {
            Id = id;
            Success = success;
            Errors.AddRange(errors);
        }
    }
}
