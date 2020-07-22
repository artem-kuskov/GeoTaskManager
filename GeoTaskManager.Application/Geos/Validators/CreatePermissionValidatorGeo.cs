using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class CreatePermissionValidator<TEntity>
        : AbstractValidator<CheckCreatePermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public CreatePermissionValidator()
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
