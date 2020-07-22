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
    internal class DbGetGeoByIdHandler
        : IRequestHandler<DbGetEntityByIdRequest<Geo>,
            EntityResponse<Geo>>
    {
        private ILogger<DbGetGeoByIdHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbGetGeoByIdHandler(ILogger<DbGetGeoByIdHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<Geo>> Handle
            (DbGetEntityByIdRequest<Geo> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetEntityByIdRequestValidator<Geo>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Request validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Request validation error");
                }

                var db = DbContext.Db;
                var dbGeoCollection = db.GetCollection<DbGeo>
                    (Defaults.GeoCollectionName).AsQueryable();
                var dbEntity = await dbGeoCollection
                    .Where(t => t.Id == request.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (dbEntity is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database null response.");
                    return ErrorResult("Database null response");
                }

                var entity = dbEntity.ToEntity<Geo>();
                return new EntityResponse<Geo>(entity);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static EntityResponse<Geo> ErrorResult
            (IEnumerable<string> messages)
            => new EntityResponse<Geo>(messages);

        private static EntityResponse<Geo> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
