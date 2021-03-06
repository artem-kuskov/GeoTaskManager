﻿using GeoTaskManager.Application.Actors.Commands;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.Actors.Mappers
{
    public static class ActorCreateCommandExtensions
    {
        public static Dictionary<string, object> ToDictionary
            (this ActorCreateCommand from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.Description), from.Description },
                { nameof(from.IsArchived),  from.IsArchived },
                { nameof(from.Title), from.Title },
                { nameof(from.CurrentPrincipal),
                    String.Join(',',
                        from.CurrentPrincipal.ToDictionary()
                            .Select(x => $"{x.Key}={x.Value}")) },
                { nameof(from.CreatedAt), from.CreatedAt },
                { nameof(from.Department), from.Department },
                { nameof(from.EMail), from.EMail },
                { nameof(from.FirstName), from.FirstName },
                { nameof(from.LastName), from.LastName },
                { nameof(from.Login), from.Login },
                { nameof(from.Phone), from.Phone },
                { nameof(from.Role), from.Role.Name },
                { nameof(from.Skype), from.Skype },
                { nameof(from.DataSeedMode), from.DataSeedMode }
            };
        }

        public static Actor ToActor(this ActorCreateCommand from,
            string creatorId)
        {
            if (from is null)
            {
                return null;
            }
            return new Actor()
            {
                CreatedAt = from.CreatedAt,
                CreatedById = creatorId,
                Description = from.Description,
                IsArchived = from.IsArchived,
                Title = from.Title,
                Department = from.Department,
                EMail = from.EMail,
                FirstName = from.FirstName,
                LastName = from.LastName,
                Login = from.Login,
                Phone = from.Phone,
                Role = from.Role,
                Skype = from.Skype
            };
        }
    }
}
