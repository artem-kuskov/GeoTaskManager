using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Security.Claims;

// Type alias
using _TEntity = GeoTaskManager.Application.Projects.Models.Project;

namespace GeoTaskManager.Application.Projects.Queries
{
    public class ProjectListQuery : IRequest<ListResponse<_TEntity>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public string СontainsKeyWords { get; set; }
        public ClaimsPrincipal CurrentPrincipal { get; set; }
    }
}
