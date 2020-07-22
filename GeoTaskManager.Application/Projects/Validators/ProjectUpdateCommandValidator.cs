using FluentValidation;

// Type alias
using _TUpdateCommand = GeoTaskManager.Application.Projects.Commands.ProjectUpdateCommand;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class ProjectUpdateCommandValidator
        : AbstractValidator<_TUpdateCommand>
    {
        public ProjectUpdateCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
