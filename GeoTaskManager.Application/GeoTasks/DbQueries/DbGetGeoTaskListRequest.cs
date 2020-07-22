using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.DbQueries
{
    public class DbGetGeoTaskListRequest : IRequest<ListResponse<GeoTask>>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public Dictionary<string, int> Actors { get; private set; }
            = new Dictionary<string, int>();
        public bool? Archived { get; set; }
        public string Contains { get; set; }
        public TimeSpan? MaxTimeToDeadLine { get; set; }
        public List<string> ProjectIds { get; private set; }
            = new List<string>();
        public List<string> GeoIds { get; private set; }
            = new List<string>();
        public int TaskStatusMask { get; set; }

        public DbGetGeoTaskListRequest Copy()
        {
            var copy = (DbGetGeoTaskListRequest)this.MemberwiseClone();
            Actors.All(x => { copy.Actors.TryAdd(x.Key, x.Value); return true; });
            ProjectIds.ForEach(x => copy.ProjectIds.Add(x));
            GeoIds.ForEach(x => copy.GeoIds.Add(x));
            return copy;
        }
    }
}
