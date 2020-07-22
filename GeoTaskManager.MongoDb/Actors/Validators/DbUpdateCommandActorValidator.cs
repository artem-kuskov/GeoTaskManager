using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.MongoDb.Core.Validators;

namespace GeoTaskManager.MongoDb.Actors.Validators
{
    internal class DbUpdateCommandActorValidator
        : AbstractValidator<DbUpdateCommand<Actor>>
    {
        public DbUpdateCommandActorValidator()
        {
            RuleFor(x => x).NotNull();
            RuleFor(x => x.Entity).NotNull();
            RuleFor(x => x.Entity.Id)
                .IsObjectId();
            RuleFor(x => x.Entity.CreatedById).IsObjectId()
                .When(x => x.Entity.CreatedById != null);
        }
    }
}
