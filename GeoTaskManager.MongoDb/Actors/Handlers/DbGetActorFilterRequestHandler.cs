using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Actors.Validators;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Extensions;
using GeoTaskManager.MongoDb.GeoTasks.Mappers;
using GeoTaskManager.MongoDb.Projects.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.Actors.Handlers
{
    internal class DbGetActorFilterRequestHandler
        : IRequestHandler<DbGetActorFilterRequest, ListResponse<Actor>>
    {
        private ILogger<DbGetActorFilterRequestHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbGetActorFilterRequestHandler
            (ILogger<DbGetActorFilterRequestHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<Actor>> Handle
            (DbGetActorFilterRequest request,
             CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetActorFilterRequestValidator()
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
                    return new ListResponse<Actor>(new List<Actor>(),
                        totalCount);
                }

                query = ApplyPagination(query, request);
                var dbActorList = await query
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (dbActorList is null)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseEmptyResponse,
                         "Database Actors null response.");
                    return ErrorResult("Database Actors null response.");
                }

                var result = new ListResponse<Actor>(dbActorList, totalCount);
                return result;
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private IMongoQueryable<Actor> BuildQuery
            (
                DbGetActorFilterRequest request
            )
        {
            var query = DbContext.Db.GetCollection<Actor>
                (Defaults.ActorCollectionName)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Contains))
            {
                query = query.WhereText(request.Contains);
            }

            if (request.Archived.HasValue)
            {
                query = query.Where(x => x.IsArchived == request.Archived);
            }

            if (request.ActorRoleMask != 0)
            {
                var roles = new HashSet<ActorRole>();
                var checkList = EnumerationClass.GetAll<ActorRole>();
                foreach (var item in checkList)
                {
                    if (request.ActorRoleMask.Contains(item))
                    {
                        roles.Add(item);
                    }
                }
                query = query.Where(x => roles.Contains(x.Role));
            }

            return query;
        }

        private IMongoQueryable<Actor> ApplyPagination(
            IMongoQueryable<Actor> query, DbGetActorFilterRequest request)
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

        private static ListResponse<Actor> ErrorResult(
            IEnumerable<string> messages)
            => new ListResponse<Actor>(messages);

        private static ListResponse<Actor> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
