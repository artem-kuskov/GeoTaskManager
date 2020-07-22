using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace GeoTaskManager.Application.GeoTasks.Queries
{
    public class GeoTaskListQuery : IRequest<ListResponse<GeoTask>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public string ProjectId { get; set; }
        public string ActorId { get; set; }
        public int ActorRoleMask { get; set; }
        public int TaskStatusMask { get; set; }
        public TimeSpan? MaxTimeToDeadLine { get; set; }
        public string СontainsKeyWords { get; set; }
        public ClaimsPrincipal CurrentPrincipal { get; set; }
        public List<string> GeoIds { get; private set; } = new List<string>();
    }
}
