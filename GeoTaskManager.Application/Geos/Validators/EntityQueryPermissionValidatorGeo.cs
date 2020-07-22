using FluentValidation;
using GeoTaskManager.Application.Core.Models;

// Type alias
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class EntityQueryPermissionValidator<TEntity>
        : AbstractValidator<CheckQueryPermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public EntityQueryPermissionValidator()
        {
            // Any actual actor can get Geos
            RuleFor(x => x.Actor)
                .NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);
            RuleFor(x => x.Entity).NotEmpty();
        }
    }
}
