using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GeoTaskManager.Application.Core.Mappers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this ClaimsPrincipal from)
        {
            if (from is null)
            {
                return null;
            }
            var result = new Dictionary<string, object>
            {
                { nameof(from.Identity.Name), from.Identity?.Name},
                {
                    nameof(from.Identity.IsAuthenticated),
                    from.Identity?.IsAuthenticated
                },
                {
                    nameof(from.Identity.AuthenticationType),
                    from.Identity?.AuthenticationType
                },
                {
                    nameof(from.Claims),
                    String.Join(',',
                        from.Claims.Select(x => $"{x.Type}={x.Value}"))},
            };
            return result;
        }
    }
}
