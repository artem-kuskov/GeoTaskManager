using FluentValidation;
using GeoTaskManager.Application.Actors.DbQueries;

namespace GeoTaskManager.MongoDb.Actors.Validators
{
    internal class DbGetActorFilterRequestValidator
        : AbstractValidator<DbGetActorFilterRequest>
    {
        public DbGetActorFilterRequestValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0).When(x => x != null);
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0).When(x => x != null);
        }
    }
}
