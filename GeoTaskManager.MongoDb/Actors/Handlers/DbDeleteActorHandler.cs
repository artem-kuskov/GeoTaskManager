using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Mappers;
using GeoTaskManager.MongoDb.Core.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.Actors.Handlers
{
    internal class DbDeleteActorHandler
        : IRequestHandler<DbDeleteCommand<Actor>,
            Application.Core.Responses.DeleteResult>
    {
        private ILogger<DbDeleteActorHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbDeleteActorHandler(ILogger<DbDeleteActorHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<Application.Core.Responses.DeleteResult> Handle
            (DbDeleteCommand<Actor> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbDeleteCommandValidator<Actor>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database command validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database command validation error.");
                }

                var db = DbContext.Db;
                var deleted = await db.GetCollection<Actor>
                    (Defaults.ActorCollectionName)
                    .FindOneAndDeleteAsync(x => x.Id == request.Id)
                    .ConfigureAwait(false);
                if (deleted is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database null response.");
                    return ErrorResult("Database null response");
                }

                return new Application.Core.Responses.DeleteResult(true);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static Application.Core.Responses.DeleteResult ErrorResult
            (IEnumerable<string> messages)
            => new Application.Core.Responses.DeleteResult(messages);

        private static Application.Core.Responses.DeleteResult ErrorResult
            (string message)
            => ErrorResult(new string[] { message });
    }
}
