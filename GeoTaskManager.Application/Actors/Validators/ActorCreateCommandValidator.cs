using FluentValidation;
using GeoTaskManager.Application.Actors.Commands;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using System.Linq;

namespace GeoTaskManager.Application.Actors.Validators
{
    public class ActorCreateCommandValidator
        : AbstractValidator<ActorCreateCommand>
    {
        public ActorCreateCommandValidator()
        {
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(xr => EnumerationClass.GetAll<ActorRole>()
                    .Any(r => r == xr));
            RuleFor(x => x.FirstName).NotEmpty();
        }
    }
}
