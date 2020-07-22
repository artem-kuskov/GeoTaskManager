using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.MongoDb.Core.Validators;
using System;

// Type alias
using _TEntity = GeoTaskManager.Application.Geos.Models.Geo;

namespace GeoTaskManager.MongoDb.Geos.Validators
{
    internal class DbCreateGeoCommandValidator
        : AbstractValidator<DbCreateCommand<_TEntity>>
    {
        public DbCreateGeoCommandValidator()
        {
            RuleFor(x => x).NotEmpty();
            RuleFor(x => x.Entity).NotNull();
            RuleFor(x => x.Entity.ProjectId)
                .IsObjectId()
                .When(x => x.Entity.ProjectId != null);
            RuleFor(x => x.Entity.GeoJson)
                .IsGeoJson()
                .When(x => !String.IsNullOrWhiteSpace(x.Entity.GeoJson));
        }
    }
}
