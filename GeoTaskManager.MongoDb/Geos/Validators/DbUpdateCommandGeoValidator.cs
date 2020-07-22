using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Core.Validators;
using System;

namespace GeoTaskManager.MongoDb.Geos.Validators
{
    internal class DbUpdateCommandGeoValidator
        : AbstractValidator<DbUpdateCommand<Geo>>
    {
        public DbUpdateCommandGeoValidator()
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
                    RuleFor(x => x.Entity.GeoJson)
                        .IsGeoJson()
                        .When(x => !String.IsNullOrWhiteSpace(x.Entity.GeoJson));
                });
        }
    }
}
