using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class CreatePermissionValidator<TEntity>
        : AbstractValidator<CheckCreatePermissionModel<TEntity>>
        where TEntity : _TEntity
    {
        public CreatePermissionValidator()
        {
            // Must have Admin or Manager global role
            RuleFor(x => x.Actor)
                .NotEmpty();
            When(x => x.Actor != null, () =>
            {
                RuleFor(x => x.Actor.IsArchived).Equal(false);
                RuleFor(x => x.Actor.Role).Must(x => x == ActorRole.Admin
                        || x == ActorRole.Manager);
            });
        }
    }
}
