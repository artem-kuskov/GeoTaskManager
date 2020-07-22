using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System;
using System.Security.Claims;

namespace GeoTaskManager.Application.Actors.Commands
{
    public class ActorUpdateCommand : IRequest<UpdateResult>
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public bool IsArchived { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string Skype { get; set; }
        public DateTime CreatedAt { get; set; }
        public ActorRole Role { get; set; }
        public ClaimsPrincipal CurrentPrincipal { get; set; }
    }
}
