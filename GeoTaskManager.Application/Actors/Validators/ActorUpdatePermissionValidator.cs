using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.Actors.Validators
{
    public class ActorUpdatePermissionValidator
        : AbstractValidator<CheckUpdatePermissionModel<Actor>>
    {
        public ActorUpdatePermissionValidator()
        {
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived).NotEqual(true);

            // Only actor with Admin global role can update actors
            RuleFor(x => x.Actor.Role).Equal(ActorRole.Admin);

            // Actor can not change himself role
            // or set soft deleted attribute
            When(x => x.Actor.Id == x.EntityBeforeUpdate.Id, () =>
                {
                    RuleFor(x => x.EntityAfterUpdate.IsArchived)
                        .Equal(x => x.EntityAfterUpdate.IsArchived);
                    RuleFor(x => x.EntityAfterUpdate.Role)
                        .Equal(x => x.EntityBeforeUpdate.Role);
                }
            );
        }
    }
}
