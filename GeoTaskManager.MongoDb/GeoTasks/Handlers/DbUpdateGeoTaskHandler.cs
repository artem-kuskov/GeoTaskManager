using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.GeoTasks.Mappers;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using GeoTaskManager.MongoDb.GeoTasks.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UpdateResult = GeoTaskManager.Application.Core.Responses.UpdateResult;


namespace GeoTaskManager.MongoDb.GeoTasks.Handlers
{
    internal class DbUpdateGeoTaskHandler
        : IRequestHandler<DbUpdateCommand<GeoTask>, UpdateResult>
    {
        private ILogger<DbUpdateGeoTaskHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbUpdateGeoTaskHandler(ILogger<DbUpdateGeoTaskHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle
            (DbUpdateCommand<GeoTask> command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", command.ToDictionary());

                var validation = new DbUpdateGeoTaskCommandValidator()
                    .Validate(command);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database command validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database command validation error.");
                }

                DbGeoTask dbEntity = command.Entity.ToDbGeoTask();

                var updated = await DbContext.Db.GetCollection<DbGeoTask>
                    (Defaults.TaskCollectionName)
                        .FindOneAndReplaceAsync(x => x.Id == dbEntity.Id,
                            dbEntity).ConfigureAwait(false);

                return new UpdateResult
                {
                    Success = !String.IsNullOrEmpty(updated?.Id),
                };
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static UpdateResult ErrorResult
            (IEnumerable<string> messages)
            => new UpdateResult(messages);

        private static UpdateResult ErrorResult(string message)
        => ErrorResult(new string[] { message });
    }
}
