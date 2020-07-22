using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

// Type alias
using _ApiEntityCreateCommandType =
    GeoTaskManager.Api.Projects.Models.ApiProjectCreateCommand;
using _EntityCreateCommandType =
    GeoTaskManager.Application.Projects.Commands.ProjectCreateCommand;


namespace GeoTaskManager.Api.Projects.Mappers
{
    internal static class ApiProjectCreateCommandExtensions
    {
        public static _EntityCreateCommandType ToEntity<TEntity>
            (this _ApiEntityCreateCommandType from,
            ClaimsPrincipal currentPrincipal)
            where TEntity : _EntityCreateCommandType
        {
            if (from is null)
            {
                return null;
            }

            var to = new _EntityCreateCommandType
            {
                Description = from.Description,
                Title = from.Title,
                CurrentPrincipal = currentPrincipal,
                IsArchived = from.IsArchived,
                IsMap = from.IsMap,
                MapProvider = from.MapProvider,
                ShowMap = from.ShowMap
            };

            to.Layers.AddRange(from.Layers);

            from.MapParameters.ToList().ForEach(x =>
                to.MapParameters.TryAdd(x.Key, x.Value));

            var allRoles = EnumerationClass.GetAll<ActorRole>();
            foreach (var item in from.ProjectActorRoles)
            {
                var actorRole = allRoles.FirstOrDefault(x =>
                                    x.Id == item.Value);
                if (actorRole != null)
                {
                    to.ProjectActorRoles.TryAdd(item.Key, actorRole);
                }
            }

            return to;
        }

        public static Dictionary<string, object> ToDictionary
            (this _ApiEntityCreateCommandType from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Layers),  String.Join(',', from.Layers.Select(x =>
                    $"{{\"{nameof(x.GeoId)}\"={x.GeoId}," +
                    $"\"{nameof(x.IsHidden)}\"={x.IsHidden}," +
                    $"\"{nameof(x.Order)}\"={x.Order}}}"))},
                { nameof(from.Description),  from.Description },
                { nameof(from.IsArchived),  from.IsArchived},
                { nameof(from.IsMap),  from.IsMap},
                { nameof(from.MapParameters),
                    String.Join(',', from.MapParameters.Select(x =>
                        $"{{\"{x.Key}\"={x.Value}}}")) },
                { nameof(from.MapProvider),  from.MapProvider},
                { nameof(from.ProjectActorRoles),
                    from.ProjectActorRoles.Select(x =>
                        $"{{\"{x.Key}\"={x.Value}}}") },
                { nameof(from.ShowMap),  from.ShowMap},
                { nameof(from.Title),  from.Title},
            };
        }
    }
}
