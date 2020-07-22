using FluentValidation;

// Type alias
using _TListQuery = GeoTaskManager.Application.Projects.Queries.ProjectListQuery;

namespace GeoTaskManager.Application.Projects.Validators
{
    public class ListQueryValidator<TEntity>
        : AbstractValidator<TEntity>
        where TEntity : _TListQuery
    {
        public ListQueryValidator()
        {
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
        }
    }
}
