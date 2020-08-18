using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.Application.GeoTasks.Models;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.MongoDb.Configuration;
using GeoTaskManager.MongoDb.Geos.Models;
using GeoTaskManager.MongoDb.GeoTasks.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoTaskManager.MongoDb
{
    public class MongoDbContext : IGeoTaskManagerDbContext
    {

        public IMongoDatabase Db { get; private set; }

        private IConfiguration Configuration { get; }
        private ILogger<MongoDbContext> Logger { get; }

        public MongoDbContext(IConfiguration configuration,
            ILogger<MongoDbContext> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        public async Task InitAsync()
        {
            var logConnectionString = false;
#if DEBUG
            logConnectionString = true;
#endif
            var connStr = Configuration.GetConnectionString
                        (Defaults.ConnectionStringParameterName)
                        ?? Defaults.ConnectionStringDefaultValue;
            var dbName = Configuration.GetConnectionString
                            (Defaults.DatabaseNameParameterName)
                            ?? Defaults.DatabaseNameDefaultValue;

            Logger.LogInformation(LogEvent.DatabaseInit, "DbName={DbName}. " +
                "ConnectionString={ConnectionString}", dbName,
                logConnectionString ? connStr : "***");

            IMongoClient client;
            IMongoDatabase db;

            try
            {
                client = new MongoClient(connStr);
                if (client is null)
                {
                    Logger.LogError(LogEvent.DatabaseInitError,
                        "Database server connection error.");
                    throw new MongoClientException
                        ("Database server connection error.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(LogEvent.DatabaseInitError, e,
                    "Database server connect exception error. Error={Error}.",
                    e.Message);
                throw new MongoClientException
                        ("Database server connect exception error.", e);
            }

            try
            {
                db = client.GetDatabase(dbName);
                if (db is null)
                {
                    Logger.LogError(LogEvent.DatabaseInitError,
                        "Get database error. DbName={DbName}", dbName);
                    throw new MongoClientException
                        ("Get database error.");
                }
                Db = db;
            }
            catch (Exception e)
            {
                Logger.LogError(LogEvent.DatabaseInitError, e,
                    "Get database exception error.");
                throw new MongoClientException
                        ("Get database exception error.", e);
            }

            try
            {
                await CreateSchemaAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(LogEvent.DatabaseInitError, e,
                    "Database Create Schema exception error.");
                throw new MongoClientException
                        ("Database Create Schema exception error.", e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        private async Task CreateSchemaAsync()
        {
            Logger.LogInformation(LogEvent.DatabaseInit,
                "Database schema creation.");

            // Map BsonDocuments and entity classes
            BsonClassMap.RegisterClassMap<Actor>(cm =>
            {
                cm.MapIdMember(p => p.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer
                                        (BsonType.ObjectId));
                cm.SetIgnoreExtraElements(true);
                cm.AutoMap();
            });
            BsonClassMap.RegisterClassMap<DbGeo>(cm =>
            {
                cm.MapIdMember(p => p.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer
                                        (BsonType.ObjectId));
                cm.SetIgnoreExtraElements(true);
                cm.AutoMap();
            });
            BsonClassMap.RegisterClassMap<DbGeoTask>(cm =>
            {
                cm.MapIdMember(p => p.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer
                                        (BsonType.ObjectId));
                cm.SetIgnoreExtraElements(true);
                cm.AutoMap();
            });
            BsonClassMap.RegisterClassMap<Project>(cm =>
            {
                cm.MapIdMember(p => p.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer
                                        (BsonType.ObjectId));
                cm.SetIgnoreExtraElements(true);
                cm.AutoMap();
            });
            BsonClassMap.RegisterClassMap<GeoTaskHistory>(cm =>
            {
                cm.MapIdMember(p => p.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer
                                        (BsonType.ObjectId));
                cm.SetIgnoreExtraElements(true);
                cm.AutoMap();
            });

            await CreateIndexes().ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        private async Task CreateIndexes()
        {
            Logger.LogInformation(LogEvent.DatabaseInit,
                "Database index creation.");

            var actorCollection = Db.GetCollection<Actor>
                                    (Defaults.ActorCollectionName);
            var actorIndexBuilder = Builders<Actor>.IndexKeys;
            var actorIndexes = new List<CreateIndexModel<Actor>>
            {
                new CreateIndexModel<Actor>(actorIndexBuilder
                                                .Ascending(x => x.CreatedAt)),
                new CreateIndexModel<Actor>(actorIndexBuilder
                                                .Ascending(x => x.IsArchived)),
                new CreateIndexModel<Actor>(
                    actorIndexBuilder.Ascending(x => x.Login),
                    new CreateIndexOptions() { Unique = true }
                ),
                new CreateIndexModel<Actor>
                (
                    actorIndexBuilder
                        .Text(x => x.LastName)
                        .Text(x=>x.FirstName)
                        .Text(x=>x.Title)
                        .Text(x=>x.Description)
                        .Text(x=>x.Department)
                        .Text(x=>x.EMail)
                        .Text(x=>x.Login)
                        .Text(x=>x.Phone)
                        .Text(x=>x.Skype)
                )
            };
            await actorCollection.Indexes.CreateManyAsync(actorIndexes)
                .ConfigureAwait(false);

            var geoCollection = Db.GetCollection<DbGeo>
                                    (Defaults.GeoCollectionName);
            var geoIndexBuilder = Builders<DbGeo>.IndexKeys;
            var geoIndexes = new List<IndexKeysDefinition<DbGeo>>
            {
                geoIndexBuilder.Ascending(x => x.CreatedAt),
                geoIndexBuilder.Ascending(x => x.CreatedBy),
                geoIndexBuilder.Ascending(x => x.IsArchived),
                geoIndexBuilder
                    .Text(x=>x.Description)
                    .Text(x=>x.Title),
                geoIndexBuilder.Geo2DSphere(
                    (FieldDefinition<DbGeo>)"GeoJson.features.geometry")
            }
            .Select(x => new CreateIndexModel<DbGeo>(x)).ToArray();
            await geoCollection.Indexes.CreateManyAsync(geoIndexes)
                .ConfigureAwait(false);

            var projectCollection = Db.GetCollection<Project>
                                        (Defaults.ProjectCollectionName);
            var projectIndexBuilder = Builders<Project>.IndexKeys;
            var projectIndexes = new List<IndexKeysDefinition<Project>>
            {
                projectIndexBuilder.Ascending(x => x.CreatedAt),
                projectIndexBuilder.Ascending(x => x.CreatedBy),
                projectIndexBuilder.Ascending(x => x.IsArchived),
                projectIndexBuilder
                    .Text(x=>x.Description)
                    .Text(x=>x.Title),
                projectIndexBuilder.Ascending(
                    (FieldDefinition<Project>)"Layers.GeoId"),

            }
            .Select(x => new CreateIndexModel<Project>(x)).ToArray();

            await projectCollection.Indexes
                .CreateManyAsync(projectIndexes)
                .ConfigureAwait(false);

            var taskCollection = Db.GetCollection<DbGeoTask>
                                    (Defaults.TaskCollectionName);
            var taskIndexBuilder = Builders<DbGeoTask>.IndexKeys;
            var taskIndexes = new List<IndexKeysDefinition<DbGeoTask>>
            {
                taskIndexBuilder.Ascending(x => x.AssistentActorsIds),
                taskIndexBuilder.Ascending(x => x.CreatedAt),
                taskIndexBuilder.Ascending(x => x.CreatedById),
                taskIndexBuilder.Ascending(x => x.IsArchived),
                taskIndexBuilder.Ascending(x => x.GeosIds),
                taskIndexBuilder.Ascending(x => x.ObserverActorsIds),
                taskIndexBuilder.Ascending(x => x.PlanFinishAt),
                taskIndexBuilder.Ascending(x => x.PlanStartAt),
                taskIndexBuilder.Ascending(x => x.ProjectId),
                taskIndexBuilder.Ascending(x => x.ResponsibleActorId),
                taskIndexBuilder.Ascending(x => x.Status),
                taskIndexBuilder
                    .Text(x=>x.Description)
                    .Text(x=>x.Title)
            }
            .Select(x => new CreateIndexModel<DbGeoTask>(x)).ToArray();

            await taskCollection.Indexes
                .CreateManyAsync(taskIndexes)
                .ConfigureAwait(false);
        }
    }
}