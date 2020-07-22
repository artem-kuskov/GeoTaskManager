using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.Actors.Validators
{
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public class ActorCreatePermissionValidator
        : AbstractValidator<CheckCreatePermissionModel<Actor>>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        public ActorCreatePermissionValidator()
        {
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);

            // Must have Admin global role
            RuleFor(x => x.Actor.Role).Must(x => x == ActorRole.Admin);
        }
    }
}
