using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Models;
using System.Linq;

namespace GeoTaskManager.Application.GeoTasks.Validators
{
    public class ActorGeoTaskRoleValidator : AbstractValidator<int>
    {
        public ActorGeoTaskRoleValidator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .Must(xr => EnumerationClass
                    .GetAll<ActorGeoTaskRole>()
                    .Any(v => v.Id == xr));
        }
    }

}
