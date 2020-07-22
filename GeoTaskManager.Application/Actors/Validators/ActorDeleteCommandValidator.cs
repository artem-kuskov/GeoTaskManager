using FluentValidation;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Commands;

namespace GeoTaskManager.Application.Actors.Validators
{
    public class ActorDeleteCommandValidator
        : AbstractValidator<DeleteCommand<Actor>>
    {
        public ActorDeleteCommandValidator()
        {
            RuleFor(x => x.CurrentPrincipal).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
