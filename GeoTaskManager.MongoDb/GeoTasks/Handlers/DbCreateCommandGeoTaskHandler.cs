using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.GeoTasks.Mappers;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using GeoTaskManager.MongoDb.GeoTasks.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.GeoTasks.Handlers
{
    internal class DbCreateCommandGeoTaskHandler
        : IRequestHandler<DbCreateCommand<GeoTask>, CreateResult>
    {
        private ILogger<DbCreateCommandGeoTaskHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbCreateCommandGeoTaskHandler
            (ILogger<DbCreateCommandGeoTaskHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle
            (DbCreateCommand<GeoTask> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                    "Request={Request}", request.ToDictionary());

                var validation = new DbCreateGeoTaskCommandValidator()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database request validation error.");
                }

                DbGeoTask dbEntity = request.Entity.ToDbGeoTask();

                await DbContext.Db.GetCollection<DbGeoTask>
                    (Defaults.TaskCollectionName)
                        .InsertOneAsync(dbEntity)
                        .ConfigureAwait(false);
                var newId = dbEntity?.Id;

                // Error while creating the entity
                if (String.IsNullOrEmpty(newId))
                {
                    return ErrorResult("Unknown error.");
                }

                return new CreateResult(newId);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static CreateResult ErrorResult(IEnumerable<string> messages)
            => new CreateResult(messages);

        private static CreateResult ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
