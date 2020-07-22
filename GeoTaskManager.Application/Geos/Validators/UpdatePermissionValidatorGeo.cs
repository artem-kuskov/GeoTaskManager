using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class UpdatePermissionValidator<TEntity>
        : AbstractValidator<CheckUpdatePermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public UpdatePermissionValidator()
        {
            // Only actor with Admin or Manager global or project role
            // can update Geos
            RuleFor(x => x.Actor)
                .NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);
            When(x => x.Actor.Role != ActorRole.Admin
                    && x.Actor.Role != ActorRole.Manager, () =>
                    {
                        RuleFor(x => x.NewProjectRole)
                            .Must(x => x == ActorRole.Admin
                                    || x == ActorRole.Manager);
                        RuleFor(x => x.OldProjectRole)
                            .Must(x => x == ActorRole.Admin
                                    || x == ActorRole.Manager);
                    }
                );
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);
            RuleFor(x => x.EntityBeforeUpdate.ProjectId)
                .Equal(x => x.EntityAfterUpdate.ProjectId);
        }
    }
}
