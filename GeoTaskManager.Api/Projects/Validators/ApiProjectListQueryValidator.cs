using FluentValidation;

// Type alias
using _ApiEntityType = GeoTaskManager.Api.Projects.Models.ApiProjectListQuery;

namespace GeoTaskManager.Api.Projects.Validators
{
    internal class ApiProjectListQueryValidator
        : AbstractValidator<_ApiEntityType>
    {
        public ApiProjectListQueryValidator()
        {
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
        }
    }
}
