namespace GeoTaskManager.Application.Projects.Models
{
    public class ProjectFilter
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public bool? Archived { get; set; }
        public string Contains { get; set; }

        public ProjectFilter Copy()
        {
            var copy = (ProjectFilter)MemberwiseClone();
            return copy;
        }
    }
}
