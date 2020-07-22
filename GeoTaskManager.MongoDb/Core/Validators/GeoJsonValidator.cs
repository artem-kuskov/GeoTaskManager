using FluentValidation;
using FluentValidation.Validators;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace GeoTaskManager.MongoDb.Core.Validators
{
    public class GeoJsonValidator : PropertyValidator
    {
        public GeoJsonValidator()
            : base("{PropertyName} must contain a valid " +
                  "GeoJSON value")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var str = context.PropertyValue as string;

            if (!String.IsNullOrWhiteSpace(str))
            {
                try
                {
                    var geoOnject = BsonSerializer
                        .Deserialize
                            <GeoJsonObject<GeoJson2DGeographicCoordinates>>
                                (str);
                    ;
                    if (geoOnject is null)
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }

    public static class GeoJsonValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsGeoJson<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            if (ruleBuilder is null)
            {
                throw new ArgumentNullException(nameof(ruleBuilder));
            }

            return ruleBuilder.SetValidator(new GeoJsonValidator());
        }
    }
}