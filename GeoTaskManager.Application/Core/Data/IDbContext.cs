using System.Threading.Tasks;

namespace GeoTaskManager.Application.Core.Data
{
    public interface IGeoTaskManagerDbContext
    {
        Task InitAsync();
    }
}
