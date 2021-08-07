
using z.ServiceProvider;
using System;
using System.ComponentModel.DataAnnotations;

namespace z.Validator.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class AutoUniqueAttribute : BaseValidationAttribute
    {
        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var objInstanceType = validationContext.ObjectInstance.GetType();

            var serviceType = typeof(IAutoUniqueValidator<>).MakeGenericType(objInstanceType);

            var service = ServiceLocator.ServiceProvider.GetService(serviceType);
            if (service == null)
                throw new ArgumentNullException($"Could not resolve an instance for service {serviceType.Name}");

            const string methodname = "IsUnique";

            var method = serviceType.GetMethod(methodname, new[] { objInstanceType, typeof(string) });
            if (method == null)
                throw new ArgumentNullException(
                    $"Service type {serviceType.Name} does not contain a method named {methodname}");

            var isUnique = (bool)method.Invoke(service, new[] { validationContext.ObjectInstance, validationContext.MemberName });

            if (isUnique)
                return ValidationResult.Success;

            ErrorMessage = "The field {0} is already exists";

            return CreateValidationErrorResult(validationContext);
        }
    }
}
