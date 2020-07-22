using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.DbQueries;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Extensions;
using GeoTaskManager.MongoDb.Projects.Mappers;
using GeoTaskManager.MongoDb.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.Projects.Handlers
{
    internal class DbGetProjectFilterRequestHandler
        : IRequestHandler<DbGetProjectFilterRequest, ListResponse<Project>>
    {
        private ILogger<DbGetProjectFilterRequestHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbGetProjectFilterRequestHandler
            (ILogger<DbGetProjectFilterRequestHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<Project>> Handle
            (DbGetProjectFilterRequest request,
             CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetProjectFilterRequestValidator()
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
                var totalCount = await query.CountAsync()
                    .ConfigureAwait(false);
                if (totalCount == 0)
                {
                    return new ListResponse<Project>(new List<Project>(),
                        totalCount);
                }

                query = ApplyPagination(query, request);
                var dbEntityList = await query
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (dbEntityList is null)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseEmptyResponse,
                         "Database Project null response.");
                    return ErrorResult("Database Project null response.");
                }

                var result = new ListResponse<Project>
                {
                    Success = true,
                    TotalCount = totalCount
                };
                result.Entities.AddRange(dbEntityList);

                return result;
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private IMongoQueryable<Project> BuildQuery
            (
                DbGetProjectFilterRequest request
            )
        {
            var query = DbContext.Db.GetCollection<Project>
                (Defaults.ProjectCollectionName)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Contains))
            {
                query = query.WhereText(request.Contains);
            }

            if (request.Archived.HasValue)
            {
                query = query.Where(x => x.IsArchived == request.Archived);
            }

            return query;
        }

        private IMongoQueryable<Project> ApplyPagination(
            IMongoQueryable<Project> query, DbGetProjectFilterRequest request)
        {
            if (request.Offset > 0)
            {
                query = query.Skip(request.Offset);
            }
            if (request.Limit > 0)
            {
                query = query.Take(request.Limit);
            }
            return query;
        }

        private static ListResponse<Project> ErrorResult(
            IEnumerable<string> messages)
            => new ListResponse<Project>(messages);

        private static ListResponse<Project> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
