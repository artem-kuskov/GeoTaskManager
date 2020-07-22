using FluentValidation;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class StatusValidator : AbstractValidator<GeoTaskStatus>
    {
        public StatusValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Must(s => EnumerationClass
                    .GetAll<GeoTaskStatus>()
                    .Any(v => v == s));
        }
    }

}
