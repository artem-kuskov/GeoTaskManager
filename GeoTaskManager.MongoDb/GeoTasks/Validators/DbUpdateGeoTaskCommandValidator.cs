using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Core.Validators;
using System.Linq;

namespace GeoTaskManager.MongoDb.GeoTasks.Validators
{
    internal class DbUpdateGeoTaskCommandValidator
        : AbstractValidator<DbUpdateCommand<GeoTask>>
    {
        public DbUpdateGeoTaskCommandValidator()
        {
            RuleFor(x => x.Entity).NotNull();
            RuleForEach(x => x.Entity.AssistentActors)
                .ChildRules(actor => actor.RuleFor(x => x.Id).IsObjectId())
                .When(x => x.Entity != null);
            RuleFor(x => x.Entity.CreatedBy.Id)
                .IsObjectId()
                .When(x => x.Entity?.CreatedBy != null);
            RuleFor(x => x.Entity.ResponsibleActor.Id)
                .IsObjectId()
                .When(x => x.Entity?.ResponsibleActor != null);
            RuleForEach(x => x.Entity.ObserverActors)
                .ChildRules(actor => actor.RuleFor(x => x.Id).IsObjectId())
                .When(x => x.Entity != null);
        }
    }
}
