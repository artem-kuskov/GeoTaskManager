using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Application.Actors.Queries;
using System.Collections.Generic;
using System.Security.Claims;

namespace GeoTaskManager.Api.Actors.Mappers
{
    internal static class ApiActorListQueryExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this ApiActorListQuery from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.ActorRoleMask), from.ActorRoleMask},
                { nameof(from.Archived), from.Archived},
                { nameof(from.ContainsKeyWords), from.ContainsKeyWords},
                { nameof(from.Limit), from.Limit},
                { nameof(from.Offset), from.Offset},
            };
        }

        public static ActorListQuery ToActorListQuery
            (this ApiActorListQuery from, ClaimsPrincipal currentPrincipal)
        {
            if (from is null)
            {
                return null;
            }

            return new ActorListQuery
            {
                ActorRoleMask = from.ActorRoleMask,
                Archived = from.Archived,
                СontainsKeyWords = from.ContainsKeyWords,
                Limit = from.Limit,
                Offset = from.Offset,
                CurrentPrincipal = currentPrincipal
            };
        }
    }
}
