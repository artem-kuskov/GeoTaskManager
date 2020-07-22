using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.GeoTasks.Models;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskCreatePermissionValidator
        : AbstractValidator<CheckCreatePermissionModel<GeoTask>>
    {
        public GeoTaskCreatePermissionValidator()
        {
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);

            // Must have Admin or Manager global role or in Project role
            RuleFor(x => x.Actor.Role)
                .Must(x => x == ActorRole.Admin || x == ActorRole.Manager)
                .When(y => y.ProjectActorRole != ActorRole.Admin
                        && y.ProjectActorRole != ActorRole.Manager);
        }
    }
}
