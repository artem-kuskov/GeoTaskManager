using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Responses
{
    public class CreateResult
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public CreateResult()
        {

        }

        public CreateResult(IEnumerable<string> errors)
        {
            Success = false;
            Errors.AddRange(errors);
        }

        public CreateResult(string id)
        {
            Success = true;
            Id = id;
        }
    }
}