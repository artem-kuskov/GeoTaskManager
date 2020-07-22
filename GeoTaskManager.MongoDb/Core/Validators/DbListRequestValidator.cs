using FluentValidation;
using GeoTaskManager.Application.Core.DbQueries;

namespace GeoTaskManager.MongoDb.Core.Validators
{
    internal class DbListRequestValidator<TEntity>
        : AbstractValidator<DbListRequest<TEntity>> where TEntity : class
    {
        public DbListRequestValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleForEach(x => x.Ids).IsObjectId();
        }
    }
}
