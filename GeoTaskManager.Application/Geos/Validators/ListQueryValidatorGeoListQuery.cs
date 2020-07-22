using FluentValidation;

// Type alias
using _TListQuery = GeoTaskManager.Application.Geos.Queries.GeoListQuery;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class ListQueryValidator<TEntity>
        : AbstractValidator<TEntity>
        where TEntity : _TListQuery
    {
        public ListQueryValidator()
        {
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CenterLong).InclusiveBetween(-180, 180);
            RuleFor(x => x.CenterLat).InclusiveBetween(-90, 90);
            RuleFor(x => x.DistanceLong).GreaterThan(0)
                .When(x => x.CenterLong != 0 || x.CenterLat != 0);
            RuleFor(x => x.DistanceLat).GreaterThan(0)
                .When(x => x.CenterLong != 0 || x.CenterLat != 0);
        }
    }
}
