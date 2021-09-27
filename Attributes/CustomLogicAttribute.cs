using z.ServiceProvider;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace z.Validator.Attributes
{
    [Obsolete("Use Throwable instead")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class, AllowMultiple = true)]
    public class CustomLogicAttribute : BaseValidationAttribute
    {
        public Type ServiceType { get; set; }
        public string MethodName { get; set; }
        
        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            var service = ServiceLocator.ServiceProvider.GetService(ServiceType);
            if (service == null)
                throw new ArgumentNullException($"Could not resolve an instance for service {ServiceType.Name}");

            var objInstanceType = validationContext.ObjectInstance.GetType();

            var method = ServiceType.GetMethod(MethodName, new[] { objInstanceType });
            if (method == null)
                throw new ArgumentNullException(
                    $"Service type {ServiceType.Name} does not contain a method named {MethodName} with argument type {objInstanceType} ");

            var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(typeof(AsyncStateMachineAttribute));

            var isValid = false;

            if (attrib != null)
            {
                var mp = (Task<bool>)method.Invoke(service, new[] { validationContext.ObjectInstance });
                isValid = mp.GetAwaiter().GetResult();
            }
            else
            {
                isValid = (bool)method.Invoke(service, new[] { validationContext.ObjectInstance });
            }

            if (isValid)
                return ValidationResult.Success;

            return CreateValidationErrorResult(validationContext);
        } 
    }
}
