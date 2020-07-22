using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.MongoDb.Projects.Validators
{
    internal class DbCreateProjectCommandValidator
        : AbstractValidator<DbCreateCommand<_TEntity>>
    {
        public DbCreateProjectCommandValidator()
        {
            RuleFor(x => x.Entity).NotNull();
        }
    }
}
