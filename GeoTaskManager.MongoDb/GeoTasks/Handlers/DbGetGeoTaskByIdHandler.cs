using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Mappers;
using GeoTaskManager.MongoDb.Core.Validators;
using GeoTaskManager.MongoDb.GeoTasks.Mappers;
using GeoTaskManager.MongoDb.GeoTasks.Models;
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
    internal class DbGetGeoTaskByIdHandler
        : IRequestHandler<DbGetEntityByIdRequest<GeoTask>,
            EntityResponse<GeoTask>>
    {
        private ILogger<DbGetGeoTaskByIdHandler> Logger { get; }
        private MongoDbContext DbContext { get; }
        private IMediator Mediator { get; }

        public DbGetGeoTaskByIdHandler(ILogger<DbGetGeoTaskByIdHandler> logger,
            IGeoTaskManagerDbContext dbContext, IMediator mediator)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
            Mediator = mediator;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<GeoTask>> Handle
            (DbGetEntityByIdRequest<GeoTask> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                    "Request={Request}.", request.ToDictionary());

                var validation = new DbGetEntityByIdRequestValidator<GeoTask>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database request validation error");
                }

                var db = DbContext.Db;
                var dbGeoTaskCollection = db.GetCollection<DbGeoTask>
                    (Defaults.TaskCollectionName).AsQueryable();
                var actorCollection = db.GetCollection<Actor>
                    (Defaults.ActorCollectionName).AsQueryable();
                var dbEntity = await dbGeoTaskCollection
                    .Where(t => t.Id == request.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (dbEntity is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database GeoTasks null response.");
                    return ErrorResult("Database GeoTasks null response");
                }

                var actorIdSet = new HashSet<string>(dbEntity.AssistentActorsIds);
                dbEntity.ObserverActorsIds.ForEach(x => actorIdSet.Add(x));
                actorIdSet.Add(dbEntity.CreatedById);
                actorIdSet.Add(dbEntity.ResponsibleActorId);
                var actorResponse = await Mediator
                    .Send(new DbListRequest<Actor>(actorIdSet))
                    .ConfigureAwait(false);

                if (!actorResponse.Success)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database Actors null response.");
                    return ErrorResult("Database Actors null response.");
                }

                var actors = actorResponse.Entities.ToDictionary(x => x.Id);
                var geoTask = dbEntity.ToGeoTask(actors);
                return new EntityResponse<GeoTask>(geoTask);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static EntityResponse<GeoTask> ErrorResult
            (IEnumerable<string> messages)
            => new EntityResponse<GeoTask>(messages);

        private static EntityResponse<GeoTask> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
