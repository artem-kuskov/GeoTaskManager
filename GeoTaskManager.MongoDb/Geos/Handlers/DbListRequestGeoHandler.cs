using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Mappers;
using GeoTaskManager.MongoDb.Core.Validators;
using GeoTaskManager.MongoDb.Geos.Mappers;
using GeoTaskManager.MongoDb.Geos.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.Geos.Handlers
{
    internal class DbListRequestGeoHandler
        : IRequestHandler<DbListRequest<Geo>, ListResponse<Geo>>
    {
        private ILogger<DbListRequestGeoHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbListRequestGeoHandler(ILogger<DbListRequestGeoHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<Geo>> Handle
            (DbListRequest<Geo> request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbListRequestValidator<Geo>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database request validation error.");
                }

                var db = DbContext.Db;
                var result = await db.GetCollection<DbGeo>
                    (Defaults.GeoCollectionName).AsQueryable()
                    .Where(t => request.Ids.Contains(t.Id))
                    .ToListAsync().ConfigureAwait(false);

                if (result is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database empty response.");
                    return ErrorResult("Database empty response");
                }

                var entities = result.Select(x => x.ToEntity<Geo>());
                return new ListResponse<Geo>(entities, result.Count);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static ListResponse<Geo> ErrorResult
            (IEnumerable<string> messages)
            => new ListResponse<Geo>(messages);

        private static ListResponse<Geo> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
