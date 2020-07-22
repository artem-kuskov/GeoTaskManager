using GeoTaskManager.Application.Core.Responses;

namespace GeoTaskManager.Application.Core.Mappers
{
    public static class DeleteResultExtensions
    {
        public static DeleteResult ToDeleteResult(this UpdateResult from)
        {
            if (from is null)
            {
                return null;
            }

            var to = new DeleteResult
            {
                Success = from.Success
            };
            to.Errors.AddRange(from.Errors);
            return to;
        }
    }
}
