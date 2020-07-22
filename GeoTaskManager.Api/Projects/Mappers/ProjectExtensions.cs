using GeoTaskManager.Api.Actors.Mappers;
using System.Linq;
using _ApiEntityType = GeoTaskManager.Api.Projects.Models.ApiProject;
using Project = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Api.Projects.Mappers
{
    internal static class ProjectExtensions
    {
        public static _ApiEntityType ToEntity<TEntity>(this Project from)
            where TEntity : _ApiEntityType
        {
            if (from is null)
            {
                return null;
            }

            var to = new _ApiEntityType
            {
                Description = from.Description,
                Id = from.Id,
                Title = from.Title,
                CreatedAt = from.CreatedAt,
                CreatedBy = from.CreatedBy.ToApiActor(),
                IsArchived = from.IsArchived,
                IsMap = from.IsMap,
                MapProvider = from.MapProvider,
                ShowMap = from.ShowMap
            };
            to.Layers.AddRange(from.Layers);
            from.MapParameters.ToList().ForEach(x =>
                to.MapParameters.TryAdd(x.Key, x.Value));
            from.ProjectActorRoles.ToList().ForEach(x =>
                to.ProjectActorRoles.TryAdd(x.Key, x.Value));

            return to;
        }
    }
}
