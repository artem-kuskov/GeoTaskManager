using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Mappers;
using GeoTaskManager.MongoDb.Core.Validators;
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
    internal class DbGetActorByIdHandler
        : IRequestHandler<DbGetEntityByIdRequest<Actor>,
            EntityResponse<Actor>>
    {
        private ILogger<DbGetActorByIdHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbGetActorByIdHandler(ILogger<DbGetActorByIdHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<Actor>> Handle
            (DbGetEntityByIdRequest<Actor> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetEntityByIdRequestValidator<Actor>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning(LogEvent.DatabaseRequestArgumentError,
                        "Request validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Request validation error");
                }

                var db = DbContext.Db;
                var result = await db.GetCollection<Actor>
                    (Defaults.ActorCollectionName)
                    .AsQueryable()
                    .Where(t => t.Id == request.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (result is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Empty response.");
                    return ErrorResult("Empty response");
                }

                return new EntityResponse<Actor>(result);
            }
            catch (Exception e)
            {
                Logger.LogError(LogEvent.DatabaseExceptionError, e,
                    "Exception error. Error={Error}.", e.Message);
                return ErrorResult("Exception error");
            }
        }

        private static EntityResponse<Actor> ErrorResult
            (IEnumerable<string> messages)
            => new EntityResponse<Actor>(messages);

        private static EntityResponse<Actor> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
