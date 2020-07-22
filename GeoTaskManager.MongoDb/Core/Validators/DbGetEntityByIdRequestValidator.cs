using FluentValidation;
using GeoTaskManager.Application.Core.DbQueries;

namespace GeoTaskManager.MongoDb.Core.Validators
{
    internal class DbGetEntityByIdRequestValidator<TEntity>
        : AbstractValidator<DbGetEntityByIdRequest<TEntity>>
        where TEntity : class
    {
        public DbGetEntityByIdRequestValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.Id).IsObjectId();
        }
    }
}
