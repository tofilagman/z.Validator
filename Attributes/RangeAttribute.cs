using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace z.Validator.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RangeAttribute : BaseValidationAttribute
    {
        public object Min { get; set; }
        public object Max { get; set; }
        public Type PropertyType { get; set; }

        public bool IncludeLowerBoundary { get; set; } = true;

        public bool IncludeUpperBoundary { get; set; } = true;

        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var minCanBeNull = Min == null;
            var maxCanBeNull = Max == null;

            // check if the property is nullable
            var propertyType = PropertyType ?? value.GetType();
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Name == "Nullable`1")
                propertyType = propertyType.GetGenericArguments()[0];

            // check if Min and Max have comparable types with the property
            Type minPropertyType;
            if (Min == null)
            {
                minCanBeNull = true;
                minPropertyType = propertyType;
            }
            else
            {
                minPropertyType = PropertyType ?? Min.GetType();
                if (minPropertyType.IsGenericType && minPropertyType.GetGenericTypeDefinition().Name == "Nullable`1")
                {
                    minCanBeNull = true;
                    minPropertyType = minPropertyType.GetGenericArguments()[0];
                }
            }

            Type maxPropertyType;
            if (Max == null)
            {
                maxCanBeNull = true;
                maxPropertyType = propertyType;
            }
            else
            {
                maxPropertyType = PropertyType ?? Max.GetType();
                if (maxPropertyType.IsGenericType && maxPropertyType.GetGenericTypeDefinition().Name == "Nullable`1")
                {
                    maxCanBeNull = true;
                    maxPropertyType = maxPropertyType.GetGenericArguments()[0];
                }
            }

            if (propertyType.GetInterface("IComparable") == null)
                throw new ArgumentException($"Type {propertyType.Name} does not implement IComparable");

            if (minPropertyType.GetInterface("IComparable") == null ||
                maxPropertyType.GetInterface("IComparable") == null)
                throw new ArgumentException("Min and/or Max parameters do not implement IComparable");


            var minConvertible = Min as IConvertible;
            var isConvertible = minConvertible == null;

            if (isConvertible)
            {
                var maxConvertible = Max as IConvertible;
                isConvertible = maxConvertible == null;
            }

            if ((propertyType != minPropertyType || propertyType != maxPropertyType) && !isConvertible)
                throw new ArgumentException("Min and/or Max parameters are of invalid type");

            var convertedValue = Convert.ChangeType(value, propertyType);

            // coalesce comparisons if Min and/or Max are null
            var compareToMin = (minCanBeNull && Min == null) ? 1
                : ((IComparable)convertedValue).CompareTo(Convert.ChangeType(Min, propertyType));

            var compareToMax = (maxCanBeNull && Max == null) ? -1
                : ((IComparable)convertedValue).CompareTo(Convert.ChangeType(Max, propertyType));

            var isValid = (IncludeLowerBoundary ? compareToMin >= 0 : compareToMin > 0) &&
                          (IncludeUpperBoundary ? compareToMax <= 0 : compareToMax < 0);

            if (isValid)
                return ValidationResult.Success;

            return CreateValidationErrorResult(validationContext);
        }
    }
}
