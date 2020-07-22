using FluentValidation;
using GeoTaskManager.Application.Core.Commands;

// Type alias
using _TEntityType = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class DeleteCommandValidator<TEntity>
        : AbstractValidator<DeleteCommand<TEntity>>
        where TEntity : _TEntityType
    {
        public DeleteCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
