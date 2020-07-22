using GeoTaskManager.Application.Core.Responses;
using MediatR;
using System.Security.Claims;

namespace GeoTaskManager.Application.Geos.Commands
{
    public class GeoCreateCommand : IRequest<CreateResult>
    {
        /// <summary>
        /// Soft delete attribute.
        ///     true - archived entity,
        ///     false - not archived entity.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Id of the Project, the Geo is linked to
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Serialized to string GeoJSON object
        /// (https://tools.ietf.org/html/rfc7946), 
        /// showing the position or area of Geo object
        /// </summary>
        public string GeoJson { get; set; }

        /// <summary>
        /// Current user identity
        /// </summary>
        public ClaimsPrincipal CurrentPrincipal { get; set; }
    }
}
