using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

namespace GeoTaskManager.Application.Actors.Validators
{
    public class ActorDeletePermissionValidator
        : AbstractValidator<CheckDeletePermissionModel<Actor>>
    {
        public ActorDeletePermissionValidator()
        {
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived).NotEqual(true);

            // Actor can not delete himself
            RuleFor(x => x.Entity.Id).NotEqual(x => x.Actor.Id);

            // Only actor with the Admin role has the permission 
            // to delete entity
            RuleFor(x => x.Actor.Role).Equal(ActorRole.Admin);
        }
    }
}
