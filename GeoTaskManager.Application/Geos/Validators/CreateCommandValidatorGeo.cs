using FluentValidation;

// Type alias
using _TCreateCommand =
    GeoTaskManager.Application.Geos.Commands.GeoCreateCommand;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class CreateCommandValidator<TEntity>
        : AbstractValidator<TEntity>
        where TEntity : _TCreateCommand
    {
        public CreateCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.GeoJson).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
