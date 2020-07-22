using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Actors.Validators;
using GeoTaskManager.MongoDb.Configuration;
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
    internal class DbGetActorByNameHandler
        : IRequestHandler<DbGetActorByNameRequest, EntityResponse<Actor>>
    {
        private ILogger<DbGetActorByNameHandler> Logger { get; }
        private MongoDbContext DbContext { get; }


        public DbGetActorByNameHandler(ILogger<DbGetActorByNameHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<Actor>> Handle
            (DbGetActorByNameRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbGetActorByNameRequestValidator()
                                        .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning(LogEvent.DatabaseRequestArgumentError,
                        "Request validation error. Error={Error}",
                        validation.Errors);
                    return ErrorResult("Request validation error.");
                }

                var db = DbContext.Db;
                var result = await db.GetCollection<Actor>
                        (Defaults.ActorCollectionName).AsQueryable()
                    .Where(t => t.Login == request.UserName)
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
