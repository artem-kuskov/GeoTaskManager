using System.Collections.Generic;
using System.Security.Claims;

// Type alias
using _ApiEntityCreateCommandType =
    GeoTaskManager.Api.Geos.Models.ApiGeoCreateCommand;
using _EntityCreateCommandType =
    GeoTaskManager.Application.Geos.Commands.GeoCreateCommand;


namespace GeoTaskManager.Api.Geos.Mappers
{
    internal static class ApiGeoCreateCommandExtensions
    {
        public static _EntityCreateCommandType ToEntity<TEntity>
            (this _ApiEntityCreateCommandType from,
            ClaimsPrincipal currentPrincipal)
            where TEntity : _EntityCreateCommandType
            => from is null
                ? null
                : new _EntityCreateCommandType
                {
                    Description = from.Description,
                    Title = from.Title,
                    CurrentPrincipal = currentPrincipal,
                    IsArchived = from.IsArchived,
                    GeoJson = from.GeoJson,
                    ProjectId = from.ProjectId
                };

        public static Dictionary<string, object> ToDictionary
            (this _ApiEntityCreateCommandType from)
            => from is null
                ? null
                : new Dictionary<string, object>
            {
                { nameof(from.Description),  from.Description },
                { nameof(from.IsArchived),  from.IsArchived},
                { nameof(from.Title),  from.Title },
                { nameof(from.GeoJson), from.GeoJson },
                { nameof(from.ProjectId), from.ProjectId }

            };
    }
}
