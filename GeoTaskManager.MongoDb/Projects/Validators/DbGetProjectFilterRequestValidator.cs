using FluentValidation;
using GeoTaskManager.Application.Projects.DbQueries;

namespace GeoTaskManager.MongoDb.Projects.Validators
{
    internal class DbGetProjectFilterRequestValidator
        : AbstractValidator<DbGetProjectFilterRequest>
    {
        public DbGetProjectFilterRequestValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0).When(x => x != null);
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0).When(x => x != null);
        }
    }
}
