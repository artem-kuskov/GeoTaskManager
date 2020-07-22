using GeoTaskManager.Application.Core.DbCommands;
using GeoTaskManager.Application.Projects.Mappers;
using GeoTaskManager.Application.Projects.Models;
using GeoTaskManager.MongoDb.Actors.Mappers;
using GeoTaskManager.MongoDb.Projects.Mappers;
using System.Collections.Generic;

namespace GeoTaskManager.MongoDb.Projects.Mappers
{
    internal static class DbUpdateCommandProjectExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this DbUpdateCommand<Project> from)
        {
            if (from is null)
            {
                return null;
            }

            var dict = new Dictionary<string, object>();

            if (from.Entity is null)
            {
                return dict;
            }

            var entityDict = from.Entity.ToDictionary();
            foreach (var item in entityDict)
            {
                dict.TryAdd(item.Key, item.Value);
            }

            return dict;
        }
    }
}
