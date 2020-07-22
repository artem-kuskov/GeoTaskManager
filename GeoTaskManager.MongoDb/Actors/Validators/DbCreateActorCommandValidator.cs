using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.DbCommands;

namespace GeoTaskManager.MongoDb.Actors.Validators
{
    internal class DbCreateActorCommandValidator
        : AbstractValidator<DbCreateCommand<Actor>>
    {
        public DbCreateActorCommandValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.Entity).NotNull().When(x => x != null);
        }
    }
}
