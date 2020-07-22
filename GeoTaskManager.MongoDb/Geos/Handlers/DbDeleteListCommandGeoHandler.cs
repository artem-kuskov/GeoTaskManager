using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Mappers;
using GeoTaskManager.MongoDb.Core.Validators;
using GeoTaskManager.MongoDb.Geos.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeleteResult = GeoTaskManager.Application.Core.Responses.DeleteResult;

namespace GeoTaskManager.MongoDb.Geos.Handlers
{
    internal class DbDeleteListCommandGeoHandler
        : IRequestHandler<DbDeleteListCommand<Geo>,
            DeleteResult>
    {
        private ILogger<DbDeleteListCommandGeoHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbDeleteListCommandGeoHandler
            (ILogger<DbDeleteListCommandGeoHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<DeleteResult> Handle
            (DbDeleteListCommand<Geo> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbDeleteListCommandValidator<Geo>()
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
                var deleted = await db.GetCollection<DbGeo>
                    (Defaults.GeoCollectionName)
                    .DeleteManyAsync(x => request.Ids.Contains(x.Id))
                    .ConfigureAwait(false);
                if (deleted is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database null response.");
                    return ErrorResult("Database null response");
                }
                else
                {
                    if (deleted.IsAcknowledged)
                    {
                        Logger.LogInformation(LogEvent.DatabaseRequest,
                            "Geo deleted count={Count}.",
                            deleted.DeletedCount);
                    }
                }
                return new DeleteResult(true);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static DeleteResult ErrorResult
            (IEnumerable<string> messages)
        => new DeleteResult(messages);

        private static DeleteResult ErrorResult(string message)
        => ErrorResult(new string[] { message });
    }
}
