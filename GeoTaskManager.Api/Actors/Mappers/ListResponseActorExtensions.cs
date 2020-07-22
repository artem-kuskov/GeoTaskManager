using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Api.Core.Models;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Responses;
using System.Linq;

namespace GeoTaskManager.Api.Actors.Mappers
{
    internal static class ListResponseActorExtensions
    {
        public static ApiList<ApiActor> ToApiActorList
            (this ListResponse<Actor> from)
        {
            var result = new ApiList<ApiActor>
            {
                TotalCount = from.TotalCount
            };
            result.Entities
                .AddRange(from.Entities
                    .Select(x => x.ToApiActor()));
            return result;
        }
    }
}
