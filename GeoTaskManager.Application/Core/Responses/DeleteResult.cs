using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Responses
{
    public class DeleteResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public DeleteResult()
        {

        }

        public DeleteResult(bool success)
        {
            Success = success;
        }

        public DeleteResult(IEnumerable<string> errors)
        {
            Errors.AddRange(errors);
            Success = false;
        }
    }
}