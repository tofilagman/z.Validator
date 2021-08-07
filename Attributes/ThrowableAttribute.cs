using z.ServiceProvider;
using System;
using System.ComponentModel.DataAnnotations;

namespace z.Validator.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ThrowableAttribute : BaseValidationAttribute
    {
        public string MethodName { protected get; set; }

        public ThrowableAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var objInstanceType = validationContext.ObjectInstance.GetType();

            var serviceType = typeof(IThrowableValidator<>).MakeGenericType(objInstanceType);
            var service = ServiceLocator.ServiceProvider.GetService(serviceType);
            if (service == null)
                throw new ArgumentNullException($"Could not resolve an instance for service {serviceType.Name}");

            var method = service.GetType().GetMethod(MethodName, new[] { objInstanceType, typeof(string) });
            if (method == null)
                throw new ArgumentNullException(
                    $"Service type {serviceType.Name} does not contain a method named {MethodName} with arguments of type {objInstanceType} and string");
            try
            {
                method.Invoke(service, new[] { validationContext.ObjectInstance, validationContext.MemberName });
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                ErrorMessage = (ex.InnerException ?? ex).Message;
                return CreateValidationErrorResult(validationContext);
            }
        }

    }
}
