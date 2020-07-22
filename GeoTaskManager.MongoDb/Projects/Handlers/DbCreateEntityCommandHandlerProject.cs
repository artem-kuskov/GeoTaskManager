using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Projects.Mappers;
using GeoTaskManager.MongoDb.Projects.Validators;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CreateResult = GeoTaskManager.Application.Core.Responses.CreateResult;

namespace GeoTaskManager.MongoDb.Projects.Handlers
{
    internal class DbCreateEntityCommandHandlerProject
        : IRequestHandler<DbCreateCommand<Project>, CreateResult>
    {
        private ILogger<DbCreateEntityCommandHandlerProject> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbCreateEntityCommandHandlerProject
            (ILogger<DbCreateEntityCommandHandlerProject> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<CreateResult> Handle
            (DbCreateCommand<Project> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                    "Request={Request}", request.ToDictionary());

                var validation = new DbCreateProjectCommandValidator()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database request validation error. " +
                        "Error={Error}.", validation.Errors);
                    return ErrorResult("Database request validation error.");
                }

                await DbContext.Db.GetCollection<Project>
                    (Defaults.ProjectCollectionName)
                        .InsertOneAsync(request.Entity)
                        .ConfigureAwait(false);
                var newId = request.Entity?.Id;

                if (String.IsNullOrWhiteSpace(newId))
                {
                    return ErrorResult("Unknown Error");
                }

                return new CreateResult(newId);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static CreateResult ErrorResult(IEnumerable<string> messages)
            => new CreateResult(messages);

        private static CreateResult ErrorResult(string message)
            => ErrorResult(new string[] { message });
    }
}
