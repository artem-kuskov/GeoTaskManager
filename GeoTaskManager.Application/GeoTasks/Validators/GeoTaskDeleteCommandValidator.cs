using FluentValidation;
using GeoTaskManager.Application.Core.Commands;
using GeoTaskManager.Application.GeoTasks.Models;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskDeleteCommandValidator
        : AbstractValidator<DeleteCommand<GeoTask>>
    {
        public GeoTaskDeleteCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
