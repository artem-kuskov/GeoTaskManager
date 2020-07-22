using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Application.Actors.Commands;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GeoTaskManager.Api.Actors.Mappers
{
    internal static class ApiActorCreateCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this ApiActorCreateCommand from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Department), from.Department},
                { nameof(from.Description), from.Description},
                { nameof(from.EMail), from.EMail},
                { nameof(from.FirstName), from.FirstName},
                { nameof(from.IsArchived), from.IsArchived},
                { nameof(from.LastName), from.LastName },
                { nameof(from.Login), from.Login },
                { nameof(from.Phone), from.Phone },
                { nameof(from.Role), from.Role },
                { nameof(from.Skype), from.Skype },
                { nameof(from.Title), from.Title }
            };
        }

        public static ActorCreateCommand ToActorCreateCommand
            (this ApiActorCreateCommand from, ClaimsPrincipal currentPrincipal)
        {
            if (from is null)
            {
                return null;
            }

            return new ActorCreateCommand
            {
                CreatedAt = DateTime.UtcNow,
                CurrentPrincipal = currentPrincipal,
                DataSeedMode = false,
                Department = from.Department,
                Description = from.Description,
                EMail = from.Description,
                FirstName = from.FirstName,
                IsArchived = from.IsArchived,
                LastName = from.LastName,
                Login = from.Login,
                Phone = from.Phone,
                Role = EnumerationClass.GetAll<ActorRole>()
                        .FirstOrDefault(x => x.Id == from.Role),
                Skype = from.Skype,
                Title = from.Title
            };
        }
    }
}
