using FluentValidation;
using GeoTaskManager.Api.GeoTasks.Models;
using System;

namespace GeoTaskManager.Api.GeoTasks.Validators
{
    internal class ApiGeoTaskListQueryValidator : AbstractValidator<ApiGeoTaskListQuery>
    {
        public ApiGeoTaskListQueryValidator()
        {
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxTimeToDeadLine).Must(
                    x => string.IsNullOrWhiteSpace(x) || TimeSpan.TryParse(x, out _)
                );
        }
    }
}
