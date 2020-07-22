using FluentValidation;
using GeoTaskManager.Application.Core.Models;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class EntityQueryPermissionValidator<TEntity>
        : AbstractValidator<CheckQueryPermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public EntityQueryPermissionValidator()
        {
            // Any actual actor can get project
            RuleFor(x => x.Actor)
                .NotEmpty();
            RuleFor(x => x.Actor.IsArchived)
                .Equal(false)
                .When(x => x.Actor != null);
            RuleFor(x => x.Entity).NotEmpty();
        }
    }
}
