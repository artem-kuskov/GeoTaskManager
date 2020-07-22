using System.Collections.Generic;
using System.Security.Claims;
using _ApiEntityType =
    GeoTaskManager.Api.Geos.Models.ApiGeoUpdateCommand;
using _EntityType =
    GeoTaskManager.Application.Geos.Commands.GeoUpdateCommand;

namespace GeoTaskManager.Api.Geos.Mappers
{
    internal static class ApiGeoUpdateCommandExtensions
    {
        public static _EntityType ToEntity<TEntity>
            (this _ApiEntityType from, string id,
            ClaimsPrincipal currentPrincipal)
            where TEntity : _EntityType
            => from is null
                ? null
                : new _EntityType
                {
                    Description = from.Description,
                    Id = id,
                    Title = from.Title,
                    CurrentPrincipal = currentPrincipal,
                    IsArchived = from.IsArchived,
                    GeoJson = from.GeoJson
                };

        public static Dictionary<string, object> ToDictionary
            (this _ApiEntityType from)
            => from is null
                ? null
                : new Dictionary<string, object>
                {
                    { nameof(from.Description),  from.Description },
                    { nameof(from.IsArchived),  from.IsArchived },
                    { nameof(from.Title),  from.Title },
                    { nameof(from.GeoJson), from.GeoJson },
                };
    }
}
