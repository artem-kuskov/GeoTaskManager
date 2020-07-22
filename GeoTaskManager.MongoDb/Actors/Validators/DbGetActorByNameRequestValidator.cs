using FluentValidation;
using GeoTaskManager.Application.Actors.DbQueries;

namespace GeoTaskManager.MongoDb.Actors.Validators
{
    internal class DbGetActorByNameRequestValidator
        : AbstractValidator<DbGetActorByNameRequest>
    {
        public DbGetActorByNameRequestValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.UserName).NotEmpty().When(x => x != null);
        }
    }
}
