using MediatR;

namespace GeoTaskManager.Application.Core.Events
{
    public class BeforeEntityDelete<TEntity> : INotification
        where TEntity : class
    {
        public TEntity Entity { get; set; }

        public BeforeEntityDelete(TEntity entity) => Entity = entity;
    }
}
