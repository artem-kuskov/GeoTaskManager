using FluentValidation;
using GeoTaskManager.Application.GeoTasks.Models;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class HistoryValidator : AbstractValidator<GeoTaskHistory>
    {
        public HistoryValidator()
        {
            RuleFor(x => x.ChangedAt).NotEmpty();
        }
    }
}
