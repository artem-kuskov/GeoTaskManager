using FluentValidation;
using GeoTaskManager.Application.GeoTasks.Commands;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskCreateCommandValidator
        : AbstractValidator<GeoTaskCreateCommand>
    {
        public GeoTaskCreateCommandValidator()
        {
            RuleFor(x => x.PlanFinishAt)
                .GreaterThanOrEqualTo(x => x.PlanStartAt)
                    .When(x => x.PlanStartAt.HasValue
                        && x.PlanFinishAt.HasValue);
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
