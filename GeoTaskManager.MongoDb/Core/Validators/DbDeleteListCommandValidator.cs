using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;

namespace GeoTaskManager.MongoDb.Core.Validators
{
    internal class DbDeleteListCommandValidator<TEntity>
        : AbstractValidator<DbDeleteListCommand<TEntity>> where TEntity : class
    {
        public DbDeleteListCommandValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleForEach(x => x.Ids).IsObjectId();
        }
    }
}
