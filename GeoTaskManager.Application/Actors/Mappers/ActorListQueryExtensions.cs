using GeoTaskManager.Application.Actors.DbQueries;
using GeoTaskManager.Application.Actors.Queries;
using GeoTaskManager.Application.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.Actors.Mappers
{
    public static class ActorListQueryExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this ActorListQuery from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                {nameof(from.ActorRoleMask), from.ActorRoleMask },
                {nameof(from.Archived), from.Archived },
                {nameof(from.Limit), from.Limit},
                {nameof(from.Offset), from.Offset },
                {nameof(from.СontainsKeyWords), from.СontainsKeyWords},
                {nameof(from.CurrentPrincipal),
                    String.Join(',', from.CurrentPrincipal.ToDictionary()
                        .Select(x => $"{x.Key}={x.Value}"))},
            };
        }

        public static DbGetActorFilterRequest ToDbGetActorFilterRequest
            (this ActorListQuery from, int defaultLimit, int maxLimit)
        {
            if (from is null)
            {
                return null;
            }

            return new DbGetActorFilterRequest
            {
                Offset = Math.Max(from.Offset, 0),
                Limit = from.Limit == 0
                    ? defaultLimit
                    : Math.Min(from.Limit, maxLimit),
                Archived = from.Archived,
                Contains = from.СontainsKeyWords,
                ActorRoleMask = from.ActorRoleMask
            };
        }
    }
}
