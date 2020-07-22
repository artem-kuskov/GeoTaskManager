using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.GeoTasks.Models;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskDeletePermissionValidator
        : AbstractValidator<CheckDeletePermissionModel<GeoTask>>
    {
        public GeoTaskDeletePermissionValidator()
        {
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);

            // Only actor with the Admin role has the permission 
            // to delete entity in hard mode
            RuleFor(x => x)
                .Must(x => x.ProjectActorRole == ActorRole.Admin
                        || x.Actor.Role == ActorRole.Admin)
                .When(x => x.HardMode == true);

            // Only actor with the Admin or Manager role has the permission 
            // to delete entity in soft mode
            RuleFor(x => x)
                .Must(x => (!x.Entity.IsArchived)
                        && (x.ProjectActorRole == ActorRole.Admin
                            || x.ProjectActorRole == ActorRole.Manager
                            || x.Actor.Role == ActorRole.Admin
                            || (x.ProjectActorRole == null
                                && x.Actor.Role == ActorRole.Manager)
                            ))
                .When(x => x.HardMode == false);
        }
    }
}
