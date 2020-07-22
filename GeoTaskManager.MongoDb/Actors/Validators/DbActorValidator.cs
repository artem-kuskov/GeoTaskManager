using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.MongoDb.Core.Validators;

namespace GeoTaskManager.MongoDb.Actors.Validators
{
    public class DbActorValidator : AbstractValidator<Actor>
    {
        public DbActorValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.Id).NotEmpty().IsObjectId();
        }
    }
}
