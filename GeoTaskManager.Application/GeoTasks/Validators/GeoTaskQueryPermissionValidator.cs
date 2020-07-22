using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskQueryPermissionValidator
        : AbstractValidator<CheckQueryPermissionModel<GeoTask>>
    {
        public GeoTaskQueryPermissionValidator()
        {

            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);

            RuleFor(x => x.Entity).NotEmpty();

            When(x => x.Actor != null && x.Entity != null, () =>
            {
                // Must have Admin, Manager, Observer global or project role
                // or participate in that task
                RuleFor(x => x)
                    .Must(y => y.Entity.AssistentActors
                            .Any(z => z.Id == y.Actor.Id)
                            || y.Entity.ObserverActors
                            .Any(z => z.Id == y.Actor.Id)
                            || y.Entity.ResponsibleActor?.Id == y.Actor.Id)
                    .When(y => (y.ProjectActorRole != ActorRole.Admin
                                    && y.ProjectActorRole != ActorRole.Manager
                                    && y.ProjectActorRole != ActorRole.Observer)
                                && (y.Actor.Role != ActorRole.Admin
                                    && y.Actor.Role != ActorRole.Manager
                                    && y.Actor.Role != ActorRole.Observer)
                         );
            });
        }
    }
}
