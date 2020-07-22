using FluentValidation;

// Type alias
using _TEntityType = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class BeforeSaveValidator<TEntity> : AbstractValidator<TEntity>
        where TEntity : _TEntityType
    {
        public BeforeSaveValidator()
        {
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.CreatedBy).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
