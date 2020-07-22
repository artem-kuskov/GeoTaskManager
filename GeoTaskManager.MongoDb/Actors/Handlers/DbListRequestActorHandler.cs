using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Responses;
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
    internal class DbListRequestActorHandler
        : IRequestHandler<DbListRequest<Actor>, ListResponse<Actor>>
    {
        private ILogger<DbListRequestActorHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbListRequestActorHandler
            (ILogger<DbListRequestActorHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<ListResponse<Actor>> Handle
            (DbListRequest<Actor> request, CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbListRequestValidator<Actor>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Request validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Request validation error.");
                }

                var db = DbContext.Db;
                var result = await db.GetCollection<Actor>
                    (Defaults.ActorCollectionName)
                    .AsQueryable()
                    .Where(t => request.Ids.Contains(t.Id))
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (result is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Empty response.");
                    return ErrorResult("Empty response");
                }

                return new ListResponse<Actor>(result, result.Count);
            }
            catch (Exception e)
            {
                Logger.LogError(LogEvent.DatabaseExceptionError, e,
                    "Exception error. Error={Error}", e.Message);
                return ErrorResult("Exception error");
            }
        }

        private static ListResponse<Actor> ErrorResult
            (IEnumerable<string> messages)
            => new ListResponse<Actor>(messages);

        private static ListResponse<Actor> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
