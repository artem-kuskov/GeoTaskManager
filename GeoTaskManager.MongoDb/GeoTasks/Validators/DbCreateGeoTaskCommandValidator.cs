﻿using FluentValidation;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Core.Validators;
using System.Linq;

namespace GeoTaskManager.MongoDb.GeoTasks.Validators
{
    internal class DbCreateGeoTaskCommandValidator
        : AbstractValidator<DbCreateCommand<GeoTask>>
    {
        public DbCreateGeoTaskCommandValidator()
        {
            RuleFor(x => x.Entity).NotNull();
            RuleForEach(x => x.Entity.AssistentActors.Select(x => x.Id))
                .IsObjectId()
                .When(x => x.Entity != null);
            RuleFor(x => x.Entity.CreatedBy.Id)
                .IsObjectId()
                .When(x => x.Entity?.CreatedBy != null);
            RuleFor(x => x.Entity.ResponsibleActor.Id)
                .IsObjectId()
                .When(x => x.Entity?.ResponsibleActor != null);
            RuleForEach(x => x.Entity.ObserverActors.Select(x => x.Id))
                .IsObjectId()
                .When(x => x.Entity != null);
        }
    }
}
