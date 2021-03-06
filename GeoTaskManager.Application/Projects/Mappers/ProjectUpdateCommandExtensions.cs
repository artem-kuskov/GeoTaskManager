﻿using GeoTaskManager.Application.Core.Mappers;
using GeoTaskManager.Application.Projects.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

// Type alias
using _TEntity =
    GeoTaskManager.Application.Projects.Commands.ProjectUpdateCommand;

namespace GeoTaskManager.Application.Projects.Mappers
{
    public static class ProjectUpdateCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this _TEntity from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Id), from.Id },
                { nameof(from.Layers),
                    String.Join(',', from.Layers.Select(x =>
                        $"{{\"{nameof(x.GeoId)}\"={x.GeoId}," +
                        $"\"{nameof(x.IsHidden)}\"={x.IsHidden}," +
                        $"\"{nameof(x.Order)}\"={x.Order}}}"))},
                { nameof(from.Description),  from.Description },
                { nameof(from.IsArchived),  from.IsArchived},
                { nameof(from.IsMap),  from.IsMap},
                { nameof(from.MapParameters),
                    String.Join(',', from.MapParameters.Select(x =>
                    $"{{\"{nameof(x.Key)}\"={x.Value}}}"))},
                { nameof(from.MapProvider),  from.MapProvider},
                { nameof(from.ProjectActorRoles),
                    String.Join(',', from.ProjectActorRoles.Select(x =>
                        $"{{\"{nameof(x.Key)}\"={x.Value}}}")) },
                { nameof(from.ShowMap),  from.ShowMap},
                { nameof(from.Title),  from.Title},
                { nameof(from.CurrentPrincipal),
                    String.Join(',', from.CurrentPrincipal
                        .ToDictionary()
                        .ToList()
                        .Select(x => $"\"{x.Key}\"={x.Value}"))
                },
            };
        }
    }
}
