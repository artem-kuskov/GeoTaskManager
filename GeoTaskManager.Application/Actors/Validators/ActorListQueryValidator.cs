using FluentValidation;
using GeoTaskManager.Application.Actors.Queries;

namespace GeoTaskManager.Application.Actors.Validators
{
    public class ActorListQueryValidator : AbstractValidator<ActorListQuery>
    {
        public ActorListQueryValidator()
        {
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
        }
    }
}
