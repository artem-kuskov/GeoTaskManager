using FluentValidation;
using GeoTaskManager.Application.Geos.DbQueries;

namespace GeoTaskManager.MongoDb.Geos.Validators
{
    internal class DbGetGeoFilterRequestValidator
        : AbstractValidator<DbGetGeoFilterRequest>
    {
        public DbGetGeoFilterRequestValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CenterLong).InclusiveBetween(-180, 180);
            RuleFor(x => x.CenterLat).InclusiveBetween(-90, 90);
            RuleFor(x => x.DistanceLat).GreaterThan(0)
                .When(x => x.CenterLat != 0 || x.CenterLong != 0);
            RuleFor(x => x.DistanceLong).GreaterThan(0)
                .When(x => x.CenterLat != 0 || x.CenterLong != 0);
        }
    }
}
