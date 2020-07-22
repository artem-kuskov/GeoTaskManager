using MediatR;

namespace GeoTaskManager.Application.Core.Events
{
    public class AfterEntityDelete<TEntity> : INotification
        where TEntity : class
    {
        public TEntity Entity { get; set; }
        public AfterEntityDelete(TEntity entity) => Entity = entity;

    }
}
