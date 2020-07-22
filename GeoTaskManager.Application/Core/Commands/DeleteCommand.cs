using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Security.Claims;

namespace GeoTaskManager.Application.Core.Commands
{
    public class DeleteCommand<TEntity>
        : IRequest<DeleteResult> where TEntity : class
    {
        public string Id { get; set; }
        public bool HardMode { get; set; }
        public ClaimsPrincipal CurrentPrincipal { get; set; }
        public string MessageTitle { get; set; }
        public string MessageDescription { get; set; }
    }
}
