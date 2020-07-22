using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using System.Linq;

namespace GeoTaskManager.Application.Actors.Validators
{
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public class ActorValidator : AbstractValidator<Actor>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        public ActorValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.LastName));
            RuleFor(x => x.LastName).NotEmpty()
                .When(x => string.IsNullOrWhiteSpace(x.FirstName));
            RuleFor(x => x.Role).NotEmpty()
                .Must(xr => EnumerationClass
                                            .GetAll<ActorRole>()
                                            .Any(r => r == xr));
        }
    }
}
