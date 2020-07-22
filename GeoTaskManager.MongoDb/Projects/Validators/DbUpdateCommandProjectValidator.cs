using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.MongoDb.Core.Validators;

namespace GeoTaskManager.MongoDb.Projects.Validators
{
    internal class DbUpdateCommandProjectValidator
        : AbstractValidator<DbUpdateCommand<Project>>
    {
        public DbUpdateCommandProjectValidator()
        {
            RuleFor(x => x.Entity).NotNull();
            When(x => x.Entity != null,
                () =>
                {
                    RuleFor(x => x.Entity.Id)
                        .IsObjectId();
                    RuleFor(x => x.Entity.CreatedBy.Id)
                        .IsObjectId()
                        .When(x => x.Entity.CreatedBy != null);
                });
        }
    }
}
