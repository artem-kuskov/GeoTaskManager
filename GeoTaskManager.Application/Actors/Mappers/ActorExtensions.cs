using GeoTaskManager.Application.Actors.Models;
using System.Collections.Generic;

namespace GeoTaskManager.Application.Actors.Mappers
{
    public static class ActorExtensions
    {
        public static Dictionary<string, object> ToDictionary(this Actor from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                {nameof(from.CreatedAt), from.CreatedAt },
                {nameof(from.Department), from.Department },
                {nameof(from.Description), from.Description },
                {nameof(from.EMail), from.EMail },
                {nameof(from.FirstName), from.FirstName },
                {nameof(from.Id), from.Id },
                {nameof(from.IsArchived), from.IsArchived },
                {nameof(from.LastName), from.LastName },
                {nameof(from.Login), from.Login },
                {nameof(from.Phone), from.Phone },
                {nameof(from.Role), from.Role.Id },
                {nameof(from.Skype), from.Skype },
                {nameof(from.Title), from.Title }
            };
        }
    }
}
