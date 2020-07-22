using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace GeoTaskManager.Application.GeoTasks.Commands
{
    public class GeoTaskCreateCommand : IRequest<CreateResult>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? PlanStartAt { get; set; }
        public DateTime? PlanFinishAt { get; set; }
        public string ProjectId { get; set; }
        public bool IsArchived { get; set; }
        public string ResponsibleActorId { get; set; }
        public List<string> AssistentActorsIds { get; } = new List<string>();
        public List<string> ObserverActorsIds { get; } = new List<string>();
        public List<string> GeosIds { get; } = new List<string>();
        public ClaimsPrincipal CurrentPrincipal { get; set; }
    }
}
