using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;

namespace GeoTaskManager.MongoDb.Core.Validators
{
    internal class DbDeleteCommandValidator<TEntity>
        : AbstractValidator<DbDeleteCommand<TEntity>> where TEntity : class
    {
        public DbDeleteCommandValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.Id).IsObjectId().When(x => x != null);
        }
    }
}
