using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Actors.Validators;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Projects.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UpdateResult = GeoTaskManager.Application.Core.Responses.UpdateResult;

namespace GeoTaskManager.MongoDb.Actors.Handlers
{
    internal class DbUpdateActorHandler
        : IRequestHandler<DbUpdateCommand<Actor>,
            UpdateResult>
    {
        private ILogger<DbUpdateActorHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbUpdateActorHandler(ILogger<DbUpdateActorHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle
            (DbUpdateCommand<Actor> command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", command.ToDictionary());

                var validation = new DbUpdateCommandActorValidator()
                    .Validate(command);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database command validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database command validation error.");
                }

                var updated = await DbContext.Db.GetCollection<Actor>
                    (Defaults.ActorCollectionName)
                        .FindOneAndReplaceAsync
                            (x => x.Id == command.Entity.Id, command.Entity)
                        .ConfigureAwait(false);

                return new UpdateResult
                {
                    Success = !string.IsNullOrEmpty(updated?.Id),
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

        private static UpdateResult ErrorResult
            (string message)
            => ErrorResult(new string[] { message });
    }
}
