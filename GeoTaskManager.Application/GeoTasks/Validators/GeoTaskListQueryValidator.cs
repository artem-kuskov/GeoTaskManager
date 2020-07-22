using FluentValidation;
using GeoTaskManager.Application.GeoTasks.Queries;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskListQueryValidator
        : AbstractValidator<GeoTaskListQuery>
    {
        public GeoTaskListQueryValidator()
        {
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
        }
    }
}
