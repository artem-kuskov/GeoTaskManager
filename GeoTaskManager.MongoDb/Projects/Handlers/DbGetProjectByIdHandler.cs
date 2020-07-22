using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbQueries;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Models;
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

namespace GeoTaskManager.MongoDb.Projects.Handlers
{
    internal class DbGetProjectByIdHandler
        : IRequestHandler<DbGetEntityByIdRequest<Project>,
            EntityResponse<Project>>
    {
        private ILogger<DbGetProjectByIdHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbGetProjectByIdHandler(ILogger<DbGetProjectByIdHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<EntityResponse<Project>> Handle
            (DbGetEntityByIdRequest<Project> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                            "Request={Request}.", request.ToDictionary());

                var validation = new DbGetEntityByIdRequestValidator<Project>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning(LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. Error={error}",
                        validation.Errors);
                    return ErrorResult("Database request validation error");
                }

                var db = DbContext.Db;
                var result = await db.GetCollection<Project>
                    (Defaults.ProjectCollectionName)
                    .AsQueryable()
                    .Where(t => t.Id == request.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                if (result is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database empty response.");
                    return ErrorResult("Database empty response");
                }

                return new EntityResponse<Project>(result);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static EntityResponse<Project> ErrorResult
            (IEnumerable<string> messages)
            => new EntityResponse<Project>(messages);

        private static EntityResponse<Project> ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
