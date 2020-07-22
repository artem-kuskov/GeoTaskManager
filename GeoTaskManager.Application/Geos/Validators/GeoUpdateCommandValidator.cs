using FluentValidation;

// Type alias
using _TUpdateCommand = GeoTaskManager.Application.Geos.Commands.GeoUpdateCommand;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class GeoUpdateCommandValidator
        : AbstractValidator<_TUpdateCommand>
    {
        public GeoUpdateCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.GeoJson).NotEmpty();
        }
    }
}
