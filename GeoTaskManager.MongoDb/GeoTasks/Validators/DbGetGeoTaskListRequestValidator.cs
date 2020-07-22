using FluentValidation;
using GeoTaskManager.Application.GeoTasks.DbQueries;
using GeoTaskManager.MongoDb.Core.Validators;

namespace GeoTaskManager.MongoDb.GeoTasks.Validators
{
    internal class DbGetGeoTaskListRequestValidator
        : AbstractValidator<DbGetGeoTaskListRequest>
    {
        public DbGetGeoTaskListRequestValidator()
        {
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleForEach(x => x.Actors.Keys).IsObjectId();
            RuleForEach(x => x.ProjectIds).IsObjectId();
            RuleForEach(x => x.GeoIds).IsObjectId();
        }
    }
}
