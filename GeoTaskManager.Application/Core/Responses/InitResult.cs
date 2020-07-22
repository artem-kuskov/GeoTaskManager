using System.Collections.Generic;

namespace GeoTaskManager.Application.Core.Responses
{
    public class InitResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; } = new List<string>();

        public InitResult()
        {

        }

        public InitResult(IEnumerable<string> errors)
        {
            if (errors != null)
            {
                Errors.AddRange(errors);
            }
            Success = false;
        }

        public InitResult(string error) : this(new List<string> { error })
        {

        }
    }
}