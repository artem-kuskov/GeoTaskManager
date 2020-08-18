using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.DbQueries;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Extensions;
using GeoTaskManager.MongoDb.GeoTasks.Mappers;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using GeoTaskManager.MongoDb.GeoTasks.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb.GeoTasks.Handlers
{
    internal class DbGetGeoTaskListHandler
        : IRequestHandler<DbGetGeoTaskListRequest, ListResponse<GeoTask>>
    {
        private ILogger<DbGetGeoTaskListHandler> Logger { get; }
        private MongoDbContext DbContext { get; }
        private IMediator Mediator { get; }

        public DbGetGeoTaskListHandler(ILogger<DbGetGeoTaskListHandler> logger,
            IGeoTaskManagerDbContext dbContext, IMediator mediator)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<GeoTask>> Handle
            (DbGetGeoTaskListRequest request,
             CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetGeoTaskListRequestValidator()
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
                    return new ListResponse<GeoTask>(new List<GeoTask>(),
                        totalCount);
                }

                query = ApplyPagination(query, request);
                var dbGeoTaskList = await query
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (dbGeoTaskList is null)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseEmptyResponse,
                         "Database GeoTasks null response.");
                    return ErrorResult("Database GeoTasks null response.");
                }

                var actorIdsSet = new HashSet<string>();
                dbGeoTaskList.ForEach
                    (x =>
                        {
                            x.AssistentActorsIds
                                .ForEach(x => actorIdsSet.Add(x));
                            actorIdsSet.Add(x.CreatedById);
                            actorIdsSet.Add(x.ResponsibleActorId);
                            x.ObserverActorsIds
                                .ForEach(x => actorIdsSet.Add(x));
                        }
                    );

                var actorListResponse = await Mediator
                    .Send(new DbListRequest<Actor>(actorIdsSet))
                    .ConfigureAwait(false);

                if (!actorListResponse.Success)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseEmptyResponse,
                         "Database Actors null response.");
                    return ErrorResult("Database Actors null response.");
                }
                var actors = actorListResponse.Entities.ToDictionary(x => x.Id);

                var result = new ListResponse<GeoTask>
                {
                    Success = true,
                    TotalCount = totalCount
                };
                dbGeoTaskList.ForEach(x =>
                        result.Entities.Add(x.ToGeoTask(actors)));

                return result;
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private IMongoQueryable<DbGeoTask> BuildQuery
            (
                DbGetGeoTaskListRequest request
            )
        {
            var query = DbContext.Db.GetCollection<DbGeoTask>
                (Defaults.TaskCollectionName)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Contains))
            {
                query = query.WhereText(request.Contains);
            }

            if (request.Archived.HasValue)
            {
                query = query.Where(x => x.IsArchived == request.Archived);
            }

            if (request.ProjectIds.Count > 0)
            {
                query = query.Where(x =>
                    request.ProjectIds.Contains(x.ProjectId));
            }

            if (request.Actors.Count > 0)
            {
                foreach (var actorPair in request.Actors)
                {
                    if (actorPair.Value == 0
                        || actorPair.Value
                            .Contains(ActorGeoTaskRole.Assistant))
                    {
                        query = query.Where(x =>
                            x.AssistentActorsIds.Any(x => x == actorPair.Key));
                    }

                    if (actorPair.Value == 0
                        || actorPair.Value.Contains(ActorGeoTaskRole.Creator))
                    {
                        query = query.Where(x =>
                            x.CreatedById == actorPair.Key);
                    }

                    if (actorPair.Value == 0
                        || actorPair.Value.Contains(ActorGeoTaskRole.Observer))
                    {
                        query = query.Where(x =>
                            x.ObserverActorsIds.Any(x => x == actorPair.Key));
                    }

                    if (actorPair.Value == 0
                        || actorPair.Value
                            .Contains(ActorGeoTaskRole.Responsible))
                    {
                        query = query.Where(x =>
                            x.ResponsibleActorId == actorPair.Key);
                    }
                }
            }

            if (request.TaskStatusMask != 0)
            {
                var statuses = new HashSet<GeoTaskStatus>();
                var checkList = EnumerationClass.GetAll<GeoTaskStatus>();
                foreach (var item in checkList)
                {
                    if (request.TaskStatusMask.Contains(item))
                    {
                        statuses.Add(item);
                    }
                }

                query = query.Where(x => statuses.Contains(x.Status));
            }

            if (request.MaxTimeToDeadLine.HasValue)
            {
                var deadlineTime = DateTime.UtcNow
                    + request.MaxTimeToDeadLine.Value;
                query = query.Where(x =>
                    x.PlanFinishAt.HasValue
                    && deadlineTime >= x.PlanFinishAt);
            }

            if (request.GeoIds.Count > 0)
            {
                query = query.Where(x =>
                    x.GeosIds.Any(z => request.GeoIds.Contains(z)));
            }

            return query;
        }

        private IMongoQueryable<DbGeoTask> ApplyPagination(
            IMongoQueryable<DbGeoTask> query, DbGetGeoTaskListRequest request)
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

        private static ListResponse<GeoTask> ErrorResult(
            IEnumerable<string> messages)
            => new ListResponse<GeoTask>(messages);

        private static ListResponse<GeoTask> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
