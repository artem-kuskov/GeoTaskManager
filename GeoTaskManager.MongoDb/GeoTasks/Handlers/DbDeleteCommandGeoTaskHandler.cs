﻿using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Core.Mappers;
using GeoTaskManager.MongoDb.Core.Validators;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeleteResult = GeoTaskManager.Application.Core.Responses.DeleteResult;

namespace GeoTaskManager.MongoDb.GeoTasks.Handlers
{
    internal class DbDeleteCommandGeoTaskHandler
        : IRequestHandler<DbDeleteCommand<GeoTask>,
            DeleteResult>
    {
        private ILogger<DbDeleteCommandGeoTaskHandler> Logger { get; }
        private MongoDbContext DbContext { get; }

        public DbDeleteCommandGeoTaskHandler
            (ILogger<DbDeleteCommandGeoTaskHandler> logger,
            IGeoTaskManagerDbContext dbContext)
        {
            Logger = logger;
            DbContext = (MongoDbContext)dbContext;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task<DeleteResult> Handle
            (DbDeleteCommand<GeoTask> request,
            CancellationToken cancellationToken)
        {
            try
            {
                Logger.LogInformation(LogEvent.DatabaseRequest,
                        "Request={Request}.", request.ToDictionary());

                var validation = new DbDeleteCommandValidator<GeoTask>()
                    .Validate(request);
                if (!validation.IsValid)
                {
                    Logger.LogWarning
                        (LogEvent.DatabaseRequestArgumentError,
                        "Database command validation error. Error={Error}.",
                        validation.Errors);
                    return ErrorResult("Database command validation error.");
                }

                var db = DbContext.Db;
                var deleted = await db.GetCollection<DbGeoTask>
                    (Defaults.TaskCollectionName)
                    .FindOneAndDeleteAsync(x => x.Id == request.Id)
                    .ConfigureAwait(false);
                if (deleted is null)
                {
                    Logger.LogWarning(LogEvent.DatabaseEmptyResponse,
                        "Database null response.");
                    return ErrorResult("Database null response");
                }

                return new DeleteResult(true);
            }
            catch (Exception e)
            {
                Logger.LogWarning(LogEvent.DatabaseExceptionError, e,
                    "Database exception error. Error={Error}.", e.Message);
                return ErrorResult("Database exception error");
            }
        }

        private static DeleteResult ErrorResult
            (IEnumerable<string> messages)
        => new DeleteResult(messages);

        private static DeleteResult ErrorResult(string message)
        => ErrorResult(new string[] { message });
    }
}
