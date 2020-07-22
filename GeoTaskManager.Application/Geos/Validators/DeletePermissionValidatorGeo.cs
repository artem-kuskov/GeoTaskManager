using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

// Type alias
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class DeletePermissionValidator<TEntity>
        : AbstractValidator<CheckDeletePermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public DeletePermissionValidator()
        {
            // Must have Admin or Manager global or project role
            RuleFor(x => x.Actor)
                .NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);
            RuleFor(x => x)
                .Must(x => x.ProjectActorRole == ActorRole.Admin
                        || x.ProjectActorRole == ActorRole.Manager)
                .When(x => x.Actor?.Role != ActorRole.Admin
                                && x.Actor?.Role != ActorRole.Manager);
        }
    }
}
