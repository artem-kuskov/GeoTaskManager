using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Responses
{
    public class UpdateResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public UpdateResult()
        {

        }

        public UpdateResult(bool success)
        {
            Success = success;
        }

        public UpdateResult(IEnumerable<string> error)
        {
            Success = false;
            Errors.AddRange(error);
        }
    }
}