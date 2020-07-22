using FluentValidation;
using GeoTaskManager.Application.Actors.Validators;
using GeoTaskManager.Application.Geos.Models;

namespace GeoTaskManager.Application.Geos.Validators
{
    public class GeoValidator : AbstractValidator<Geo>
    {
        public GeoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.CreatedAt).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.CreatedBy).SetValidator(new ActorValidator());
            RuleFor(x => x.GeoJson).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();
        }
    }
}
