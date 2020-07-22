using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Geos.Mappers;
using GeoTaskManager.MongoDb.Geos.Models;
using GeoTaskManager.MongoDb.Geos.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.Geos.Handlers
{
    internal class DbCreateCommandGeoHandler
        : IRequestHandler<DbCreateCommand<Geo>, CreateResult>
    {
        private ILogger<DbCreateCommandGeoHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbCreateCommandGeoHandler
            (ILogger<DbCreateCommandGeoHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle
            (DbCreateCommand<Geo> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                    "Request={Request}", request.ToDictionary());

                var validation = new DbCreateGeoCommandValidator()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. " +
                        "Error={Error}.", validation.Errors);
                    return ErrorResult("Database request validation error.");
                }

                var dbGeo = request.Entity?.ToEntity<DbGeo>();

                await DbContext.Db.GetCollection<DbGeo>
                    (Defaults.GeoCollectionName)
                        .InsertOneAsync(dbGeo)
                        .ConfigureAwait(false);
                var newId = dbGeo?.Id;
                if (String.IsNullOrEmpty(newId))
                {
                    return ErrorResult("Unknown Error");
                }
                return new CreateResult(id: newId);
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
