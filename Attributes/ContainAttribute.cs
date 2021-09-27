using System;
using System.ComponentModel.DataAnnotations;
using System.Linq; 

namespace z.Validator.Attributes
{ 
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ContainAttribute : BaseValidationAttribute
    {
        public object[] Values { get; set; }
       
        public Type PropertyType { get; set; }

        /// <summary>
        /// sets to true when the list is included or excluded
        /// </summary>
        public bool Include { get; set; } = true;
         
        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
             
            // check if the property is nullable
            var propertyType = PropertyType ?? value.GetType();
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Name == "Nullable`1")
                propertyType = propertyType.GetGenericArguments()[0];
             
            if (propertyType.GetInterface("IComparable") == null)
                throw new ArgumentException($"Type {propertyType.Name} does not implement IComparable");
              
            var convertedValue = Convert.ChangeType(value, propertyType);
              
            if (Values.Any(x => Convert.ChangeType(x, propertyType).Equals(convertedValue)) == Include)
                return ValidationResult.Success;

            return CreateValidationErrorResult(validationContext);
        }
    }
}
