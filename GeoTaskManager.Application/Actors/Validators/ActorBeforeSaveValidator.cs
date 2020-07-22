using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using System.Linq;

namespace GeoTaskManager.Application.Actors.Validators
{
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public class ActorBeforeSaveValidator : AbstractValidator<Actor>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        public ActorBeforeSaveValidator()
        {
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(xr => EnumerationClass
                                            .GetAll<ActorRole>()
                                            .Any(r => r == xr));
        }
    }
}
