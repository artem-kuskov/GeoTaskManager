using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.GeoTasks.Models;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class GeoTaskUpdatePermissionValidator
        : AbstractValidator<CheckUpdatePermissionModel<GeoTask>>
    {
        public GeoTaskUpdatePermissionValidator()
        {
            RuleFor(x => x.Actor).NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);

            //Actor with role Observer has no permission to change anything
            RuleFor(x => x)
                .Must(x => (
                                x.NewProjectRole == ActorRole.Actor
                                || x.NewProjectRole == ActorRole.Admin
                                || x.NewProjectRole == ActorRole.Manager
                           )
                           &&
                           (
                                x.OldProjectRole == ActorRole.Actor
                                || x.OldProjectRole == ActorRole.Admin
                                || x.OldProjectRole == ActorRole.Manager
                           )
                     )
                .When(x => x.Actor.Role == ActorRole.Observer);
            RuleFor(x => x)
                .Must(x => (x.NewProjectRole != ActorRole.Observer)
                           &&
                           (x.OldProjectRole != ActorRole.Observer))
                .When(x => x.Actor.Role != ActorRole.Observer);

            // Actor without global or project role of Admin or Manager has 
            // no permission to change anything but the next statuses:
            //      New, InWork, CancelRequested => FinishRequested
            //      New, InWork, FinishRequested => CancelRequested
            //      New, FinishRequested, CancelRequested => InWork
            //      InWork, FinishRequested, CancelRequested => New
            When(x => x.Actor.Role != ActorRole.Admin
                &&
                x.Actor.Role != ActorRole.Manager
                && (
                       (x.OldProjectRole != ActorRole.Admin && x.OldProjectRole != ActorRole.Manager)
                    || (x.NewProjectRole != ActorRole.Admin && x.NewProjectRole != ActorRole.Manager)
                ),
                () =>
                {
                    RuleFor(x => x)
                        .Must(x => x.EntityAfterUpdate.AssistentActors
                            .All(c => x.EntityBeforeUpdate.AssistentActors
                                .Contains(c)));
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.AssistentActors
                            .All(c => x.EntityAfterUpdate.AssistentActors
                                .Contains(c)));
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.CreatedAt ==
                            x.EntityAfterUpdate.CreatedAt);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.CreatedBy ==
                            x.EntityAfterUpdate.CreatedBy);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.Description ==
                            x.EntityAfterUpdate.Description);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.GeosIds
                            .All(c => x.EntityAfterUpdate.GeosIds
                                .Contains(c)));
                    RuleFor(x => x)
                        .Must(x => x.EntityAfterUpdate.GeosIds
                            .All(c => x.EntityBeforeUpdate.GeosIds
                                .Contains(c)));
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.Id ==
                            x.EntityAfterUpdate.Id);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.IsArchived ==
                                    x.EntityAfterUpdate.IsArchived);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.ObserverActors
                            .All(c => x.EntityAfterUpdate.ObserverActors
                                .Contains(c)));
                    RuleFor(x => x)
                       .Must(x => x.EntityAfterUpdate.ObserverActors
                           .All(c => x.EntityBeforeUpdate.ObserverActors
                               .Contains(c)));
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.PlanFinishAt ==
                            x.EntityAfterUpdate.PlanFinishAt);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.PlanStartAt ==
                            x.EntityAfterUpdate.PlanStartAt);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.ProjectId ==
                            x.EntityAfterUpdate.ProjectId);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.ResponsibleActor ==
                            x.EntityAfterUpdate.ResponsibleActor);
                    RuleFor(x => x)
                        .Must(x => x.EntityBeforeUpdate.Title ==
                            x.EntityAfterUpdate.Title);
                    RuleFor(x => x)
                        .Must(x => (x.EntityAfterUpdate.Status ==
                                        x.EntityAfterUpdate.Status
                                        &&
                                        x.EntityAfterUpdate.StatusChangedAt ==
                                        x.EntityBeforeUpdate.StatusChangedAt
                                   )
                                   ||
                                   (x.EntityAfterUpdate.Status
                                    == GeoTaskStatus.FinishRequested
                                        && (x.EntityBeforeUpdate.Status
                                        == GeoTaskStatus.New
                                            || x.EntityBeforeUpdate.Status
                                            == GeoTaskStatus.InWork
                                            || x.EntityBeforeUpdate.Status
                                            == GeoTaskStatus.CancelRequested)
                                   )
                                   ||
                                   (x.EntityAfterUpdate.Status
                                    == GeoTaskStatus.CancelRequested
                                        && (x.EntityBeforeUpdate.Status
                                            == GeoTaskStatus.New
                                            || x.EntityBeforeUpdate.Status
                                                == GeoTaskStatus.InWork
                                            || x.EntityBeforeUpdate.Status
                                                == GeoTaskStatus.FinishRequested)
                                   )
                                   ||
                                   (x.EntityAfterUpdate.Status
                                    == GeoTaskStatus.InWork
                                        && (x.EntityBeforeUpdate.Status
                                            == GeoTaskStatus.New
                                            || x.EntityBeforeUpdate.Status
                                                == GeoTaskStatus.FinishRequested
                                            || x.EntityBeforeUpdate.Status
                                                == GeoTaskStatus.CancelRequested)
                                   )
                                   ||
                                   (x.EntityAfterUpdate.Status
                                    == GeoTaskStatus.New
                                        && (x.EntityBeforeUpdate.Status
                                            == GeoTaskStatus.InWork
                                            || x.EntityBeforeUpdate.Status
                                                == GeoTaskStatus.FinishRequested
                                            || x.EntityBeforeUpdate.Status
                                                == GeoTaskStatus.CancelRequested)
                                   )

                        );
                });
        }
    }
}