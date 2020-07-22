using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GeoTaskManager.Api.Core.Filters
{
    internal class ApiValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync
            (ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorResponse = new ApiErrorResponse();

                // Add errors from ModelState to ErrorResponse

                var errorList = context.ModelState
                    .Where(state => state.Value.Errors.Count > 0)
                    .SelectMany
                    (
                        state => state.Value.Errors
                            .Select(err =>
                                new ApiErrorModel(state.Key, err.ErrorMessage))
                    )
                    .ToList();

                errorResponse.Errors.AddRange(errorList);

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }
            _ = await next().ConfigureAwait(false);
        }
    }
}
