using System;
using System.ComponentModel.DataAnnotations;

namespace z.Validator.Attributes
{
    /// <summary>
    /// Custom required validation attribute, allowing to return a database resource as localized error message
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class IsRequiredAttribute : BaseValidationAttribute
    {
        public int NumericValue { get; set; } = 0;

        public IsRequiredAttribute()
        {
            Key = "ValueRequired";
        }

        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            ErrorMessage = "{0} is required";

            if (string.IsNullOrEmpty(value?.ToString()))
                return CreateValidationErrorResult(validationContext);
             
            if (typeof(int) == value.GetType())
                if (int.TryParse(value.ToString(), out int s))
                    if (s <= NumericValue)
                        return CreateValidationErrorResult(validationContext);

            if (typeof(long) == value.GetType())
                if (long.TryParse(value.ToString(), out long s))
                    if (s <= NumericValue)
                        return CreateValidationErrorResult(validationContext);

            if (typeof(double) == value.GetType() && double.TryParse(value.ToString(), out double result) && result <= NumericValue)
            {
                return CreateValidationErrorResult(validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
