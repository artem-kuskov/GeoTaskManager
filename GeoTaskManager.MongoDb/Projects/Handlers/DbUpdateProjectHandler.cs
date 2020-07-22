using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Projects.Mappers;
using GeoTaskManager.MongoDb.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UpdateResult = GeoTaskManager.Application.Core.Responses.UpdateResult;

namespace GeoTaskManager.MongoDb.Projects.Handlers
{
    internal class DbUpdateProjectHandler
        : IRequestHandler<DbUpdateCommand<Project>, UpdateResult>
    {
        private ILogger<DbUpdateProjectHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbUpdateProjectHandler(ILogger<DbUpdateProjectHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<UpdateResult> Handle
            (DbUpdateCommand<Project> command,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", command.ToDictionary());

                var validation = new DbUpdateCommandProjectValidator()
                    .Validate(command);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database command validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database command validation error.");
                }

                var updated = await DbContext.Db.GetCollection<Project>
                    (Defaults.ProjectCollectionName)
                        .FindOneAndReplaceAsync
                            (x => x.Id == command.Entity.Id, command.Entity)
                        .ConfigureAwait(false);

                if (string.IsNullOrEmpty(updated?.Id))
                {
                    return ErrorResult("Unknown error");
                }

                return new UpdateResult(success: true);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static UpdateResult ErrorResult(IEnumerable<string> messages)
            => new UpdateResult(messages);

        private static UpdateResult ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
