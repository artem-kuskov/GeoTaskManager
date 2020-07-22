using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.Projects.Models;
using MediatR;

namespace GeoTaskManager.Application.Projects.DbQueries
{
    public class DbGetProjectFilterRequest : IRequest<ListResponse<Project>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public string Contains { get; set; }

        public DbGetProjectFilterRequest Copy()
        {
            var copy = (DbGetProjectFilterRequest)MemberwiseClone();
            return copy;
        }
    }
}
