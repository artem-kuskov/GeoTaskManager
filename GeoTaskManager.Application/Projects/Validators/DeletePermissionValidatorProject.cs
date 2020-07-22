using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class DeletePermissionValidator<TEntity>
        : AbstractValidator<CheckDeletePermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public DeletePermissionValidator()
        {
            // Only actor with the Admin or Manager role has the permission 
            // to delete entity
            RuleFor(x => x.Actor)
                .NotEmpty();
            When(x => x.Actor != null, () =>
            {
                RuleFor(x => x.Actor.IsArchived).Equal(false);
                RuleFor(x => x.Actor.Role)
                    .Must(x => x == ActorRole.Admin
                             | x == ActorRole.Manager);
            }
            );
        }
    }
}
