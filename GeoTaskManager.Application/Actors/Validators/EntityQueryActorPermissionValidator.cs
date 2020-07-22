using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.Actors.Validators
{
    public class EntityQueryActorPermissionValidator
        : AbstractValidator<CheckQueryPermissionModel<Actor>>
    {
        public EntityQueryActorPermissionValidator()
        {

            // Any actual actor can get other actor
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);
            RuleFor(x => x.Entity).NotEmpty();
        }
    }
}
