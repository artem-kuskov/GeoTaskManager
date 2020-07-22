using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Geos.Mappers;
using GeoTaskManager.MongoDb.Geos.Models;
using GeoTaskManager.MongoDb.Geos.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UpdateResult = GeoTaskManager.Application.Core.Responses.UpdateResult;

namespace GeoTaskManager.MongoDb.Geos.Handlers
{
    internal class DbUpdateGeoHandler
        : IRequestHandler<DbUpdateCommand<Geo>, UpdateResult>
    {
        private ILogger<DbUpdateGeoHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbUpdateGeoHandler(ILogger<DbUpdateGeoHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle
            (DbUpdateCommand<Geo> command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", command.ToDictionary());

                var validation = new DbUpdateCommandGeoValidator()
                    .Validate(command);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database command validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database command validation error.");
                }

                var dbGeo = command.Entity.ToEntity<DbGeo>();

                var updated = await DbContext.Db.GetCollection<DbGeo>
                    (Defaults.GeoCollectionName)
                        .FindOneAndReplaceAsync
                            (x => x.Id == dbGeo.Id, dbGeo)
                        .ConfigureAwait(false);

                if (string.IsNullOrEmpty(updated?.Id))
                {
                    return ErrorResult("Unknown Error.");
                }

                return new UpdateResult(success: true);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static UpdateResult ErrorResult(IEnumerable<string> messages)
            => new UpdateResult(messages);

        private static UpdateResult ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
