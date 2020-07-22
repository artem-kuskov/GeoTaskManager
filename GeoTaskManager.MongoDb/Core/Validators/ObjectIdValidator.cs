using FluentValidation;
using FluentValidation.Validators;
using MongoDB.Bson;
using System;

namespace GeoTaskManager.MongoDb.Core.Validators
{
    public class ObjectIdValidator : PropertyValidator
    {
        public ObjectIdValidator()
            : base("{PropertyName} must contain a valid " +
                  "MongoDb ObjectId value")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var str = context.PropertyValue as string;

            if (!string.IsNullOrWhiteSpace(str))
            {
                return ObjectId.TryParse(str, out _);
            }

            return false;
        }
    }

    public static class ObjectIdValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsObjectId<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            if (ruleBuilder is null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new ObjectIdValidator());
        }
    }
}