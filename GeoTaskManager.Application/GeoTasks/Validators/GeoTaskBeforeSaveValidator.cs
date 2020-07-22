using FluentValidation;
using GeoTaskManager.Application.GeoTasks.Models;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskBeforeSaveValidator : AbstractValidator<GeoTask>
    {
        public GeoTaskBeforeSaveValidator()
        {
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleForEach(x => x.History).SetValidator(new HistoryValidator());
            RuleFor(x => x.PlanFinishAt)
                .GreaterThanOrEqualTo(x => x.PlanStartAt)
                    .When(x => x.PlanStartAt.HasValue
                        && x.PlanFinishAt.HasValue);
            RuleFor(x => x.Status).SetValidator(new StatusValidator());
            RuleFor(x => x.StatusChangedAt).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
