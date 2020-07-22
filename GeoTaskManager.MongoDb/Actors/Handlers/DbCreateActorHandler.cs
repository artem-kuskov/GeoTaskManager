using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Actors.Validators;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.GeoTasks.Mappers;
using GeoTaskManager.MongoDb.Projects.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.Actors.Handlers
{
    public class DbCreateActorHandler
        : IRequestHandler<DbCreateCommand<Actor>, CreateResult>
    {
        private ILogger<DbCreateActorHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbCreateActorHandler(ILogger<DbCreateActorHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle
            (DbCreateCommand<Actor> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                    "Request={Request}", request.ToDictionary());

                var validation = new DbCreateActorCommandValidator()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. " +
                        "Error={Error}.", validation.Errors);
                    return ErrorResult("Database request validation error.");
                }

                await DbContext.Db.GetCollection<Actor>
                    (Defaults.ActorCollectionName)
                        .InsertOneAsync(request.Entity)
                        .ConfigureAwait(false);
                var newId = request.Entity?.Id;
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
