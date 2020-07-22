using FluentValidation;

// Type alias
using _TCreateCommand = GeoTaskManager.Application.Projects.Commands.ProjectCreateCommand;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class CreateCommandValidator<TEntity>
        : AbstractValidator<TEntity>
        where TEntity : _TCreateCommand
    {
        public CreateCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
