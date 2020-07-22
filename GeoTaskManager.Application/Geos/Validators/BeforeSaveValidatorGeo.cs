using FluentValidation;

// Type alias
using _TEntityType = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class BeforeSaveValidator<TEntity> : AbstractValidator<TEntity>
        where TEntity : _TEntityType
    {
        public BeforeSaveValidator()
        {
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.CreatedBy).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.GeoJson).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
