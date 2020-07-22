using GeoTaskManager.Api.Actors.Models;
using GeoTaskManager.Application.Actors.Models;

namespace GeoTaskManager.Api.Actors.Mappers
{
    internal static class ActorExtensions
    {
        public static ApiActor ToApiActor(this Actor from)
        {
            if (from is null)
            {
                return null;
            }

            return new ApiActor
            {
                Description = from.Description,
                Id = from.Id,
                Title = from.Title,
                CreatedAt = from.CreatedAt,
                EMail = from.EMail,
                FirstName = from.FirstName,
                Department = from.Department,
                IsArchived = from.IsArchived,
                LastName = from.LastName,
                Login = from.Login,
                Phone = from.Phone,
                Skype = from.Skype,
                Role = from.Role
            };
        }
    }
}
