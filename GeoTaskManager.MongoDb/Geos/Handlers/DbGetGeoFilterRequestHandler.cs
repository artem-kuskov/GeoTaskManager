using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Geos.DbQueries;
using GeoTaskManager.Application.Geos.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Extensions;
using GeoTaskManager.MongoDb.Geos.Mappers;
using GeoTaskManager.MongoDb.Geos.Models;
using GeoTaskManager.MongoDb.Geos.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Type alias
using _2DCoord = MongoDB.Driver.GeoJsonObjectModel.GeoJson2DGeographicCoordinates;

namespace GeoTaskManager.MongoDb.Geos.Handlers
{
    internal class DbGetGeoFilterRequestHandler
        : IRequestHandler<DbGetGeoFilterRequest, ListResponse<Geo>>
    {
        private const int EarthRadiusMeters = 6378100;

        private ILogger<DbGetGeoFilterRequestHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbGetGeoFilterRequestHandler
            (ILogger<DbGetGeoFilterRequestHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<Geo>> Handle
            (DbGetGeoFilterRequest request,
             CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetGeoFilterRequestValidator()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                         "Database request validation" +
                         " error. Error={Error}.", validation.Errors);
                    return ErrorResult("Database request validation error");
                }

                var db = DbContext.Db;
                var query = BuildQuery(request);
                var totalCount = await query
                    .CountAsync()
                    .ConfigureAwait(false);
                if (totalCount == 0)
                {
                    return new ListResponse<Geo>(new List<Geo>(),
                        totalCount);
                }

                var pagedQuery = AddPagination(query, request);
                var dbEntityList = await pagedQuery
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (dbEntityList is null)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseEmptyResponse,
                         "Database Geo null response.");
                    return ErrorResult("Database Geo null response.");
                }

                var entities = dbEntityList.Select(x => x.ToEntity<Geo>());
                var result = new ListResponse<Geo>(entities, totalCount);

                return result;
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private IMongoQueryable<DbGeo> BuildQuery
            (
                DbGetGeoFilterRequest request
            )
        {

            var query = DbContext.Db.GetCollection<DbGeo>
                    (Defaults.GeoCollectionName)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Contains))
            {
                query = query.WhereText(request.Contains);
            }

            if (request.Archived.HasValue)
            {
                query = query.Where(x => x.IsArchived == request.Archived);
            }

            if (!string.IsNullOrWhiteSpace(request.ProjectId))
            {
                query = query.Where(x => x.ProjectId == request.ProjectId);
            }

            // Add geospatial filter
            if (request.DistanceLat > 0 && request.DistanceLong > 0)
            {
                var polygon = BuildLimitBox(request);
                var filterBuilder = Builders<DbGeo>.Filter;
                var filter = filterBuilder
                    .GeoIntersects<GeoJson2DGeographicCoordinates>
                        ((FieldDefinition<DbGeo>)"GeoJson.features.geometry",
                        polygon);
                query = query.Where(x => filter.Inject());
            }
            return query;

            static GeoJsonPolygon<GeoJson2DGeographicCoordinates> BuildLimitBox
                (DbGetGeoFilterRequest request)
            {
                double yShift = (180 / Math.PI) *
                    (request.DistanceLat / EarthRadiusMeters);
                double xShift = (180 / Math.PI) *
                    (request.DistanceLong / EarthRadiusMeters) /
                    Math.Cos(request.CenterLat);
                var polygon = new GeoJsonPolygon<_2DCoord>(
                    new GeoJsonPolygonCoordinates<_2DCoord>(
                        new GeoJsonLinearRingCoordinates<_2DCoord>(
                            new List<_2DCoord>
                            {
                                new _2DCoord(request.CenterLong - xShift,
                                    request.CenterLat - yShift),
                                new _2DCoord(request.CenterLong + xShift,
                                    request.CenterLat - yShift),
                                new _2DCoord(request.CenterLong + xShift,
                                    request.CenterLat + yShift),
                                new _2DCoord(request.CenterLong - xShift,
                                    request.CenterLat + yShift),
                                new _2DCoord(request.CenterLong - xShift,
                                    request.CenterLat - yShift),
                            })));
                return polygon;
            }
        }

        private IMongoQueryable<DbGeo> AddPagination
            (IMongoQueryable<DbGeo> query, DbGetGeoFilterRequest request)
        {
            var newQuery = query;
            if (request.Offset > 0)
            {
                newQuery = newQuery.Skip(request.Offset);
            }
            if (request.Limit > 0)
            {
                newQuery = newQuery.Take(request.Limit);
            }
            return newQuery;
        }

        private static ListResponse<Geo> ErrorResult(
            IEnumerable<string> messages)
            => new ListResponse<Geo>(messages);

        private static ListResponse<Geo> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
