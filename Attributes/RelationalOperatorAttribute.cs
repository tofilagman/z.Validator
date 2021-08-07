using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace z.Validator.Attributes
{
    /// <summary>
    /// Allows to define a validation rule based on a relational comparison operation with another property.
    /// The two properties must be of comparable types, i.e. T and/or Nullable{T}. Furthermore, T must implement IComparable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class RelationalOperatorAttribute : BaseValidationAttribute
    {
        /// <summary>
        /// Defines the relational operato to apply when validating the property
        /// </summary>
        public RelationalOperators Operator { get; set; }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string OtherProperty { get; set; }

        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var otherPropertyCanBeNull = false;

            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
                throw new ArgumentException($"Property {OtherProperty} does not exist");

            // verify that the two property types are comparable and if they are nullable
            var propertyType = value.GetType();
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Name == "Nullable`1")
                propertyType = propertyType.GetGenericArguments()[0];

            var otherPropertyType = otherPropertyInfo.PropertyType;
            if (otherPropertyType.IsGenericType && otherPropertyType.GetGenericTypeDefinition().Name == "Nullable`1")
            {
                otherPropertyCanBeNull = true;
                otherPropertyType = otherPropertyType.GetGenericArguments()[0];
            }

            // check whether the two property types are the same (or nullable) and whether they are comparable
            if (propertyType.GetInterface("IComparable") == null)
                throw new ArgumentException($"Type {propertyType.Name} does not implement IComparable");
            if (propertyType != otherPropertyType)
                throw new ArgumentException(
                    $"Properties {validationContext.MemberName} and {OtherProperty} do not have comparable types");


            // check if other property is nullable and its value is null
            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if (otherPropertyCanBeNull && otherValue == null)
                return ValidationResult.Success;

            // finally compare the two values
            var operand1 = (IComparable)value;
            var operand2 = (IComparable)otherValue;
            int comparison = operand1.CompareTo(operand2);

            var isValid =
                (comparison < 0 &&
                    (Operator == RelationalOperators.LessThan || Operator == RelationalOperators.LessThanOrEqualTo || Operator == RelationalOperators.NotEqualTo)) ||
                (comparison > 0 &&
                    (Operator == RelationalOperators.GreaterThan || Operator == RelationalOperators.GreaterThanOrEqualTo || Operator == RelationalOperators.NotEqualTo)) ||
                (comparison == 0 &&
                    (Operator == RelationalOperators.EqualTo || Operator == RelationalOperators.GreaterThanOrEqualTo || Operator == RelationalOperators.LessThanOrEqualTo));

            if (isValid)
                return ValidationResult.Success;
             
            return CreateValidationErrorResult(validationContext, OtherProperty);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class OperatorAttribute : BaseValidationAttribute
    {
        /// <summary>
        /// Defines the relational operato to apply when validating the property
        /// </summary>
        public RelationalOperators Operator { get; set; }

        /// <summary>
        /// Name of the property
        /// </summary>
        public object CompareValue { get; set; }

        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var otherPropertyCanBeNull = false;
             
            // verify that the two property types are comparable and if they are nullable
            var propertyType = value.GetType();
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition().Name == "Nullable`1")
                propertyType = propertyType.GetGenericArguments()[0];

            
            // check whether the two property types are the same (or nullable) and whether they are comparable
            if (propertyType.GetInterface("IComparable") == null)
                throw new ArgumentException($"Type {propertyType.Name} does not implement IComparable");
           
            // check if other property is nullable and its value is null 
            if (otherPropertyCanBeNull)
                return ValidationResult.Success;

            // finally compare the two values
            var operand1 = (IComparable)value;
            var operand2 = (IComparable)CompareValue;
            int comparison = operand1.CompareTo(operand2);

            var isValid =
                (comparison < 0 &&
                    (Operator == RelationalOperators.LessThan || Operator == RelationalOperators.LessThanOrEqualTo || Operator == RelationalOperators.NotEqualTo)) ||
                (comparison > 0 &&
                    (Operator == RelationalOperators.GreaterThan || Operator == RelationalOperators.GreaterThanOrEqualTo || Operator == RelationalOperators.NotEqualTo)) ||
                (comparison == 0 &&
                    (Operator == RelationalOperators.EqualTo || Operator == RelationalOperators.GreaterThanOrEqualTo || Operator == RelationalOperators.LessThanOrEqualTo));

            if (isValid)
                return ValidationResult.Success;

            return CreateValidationErrorResult(validationContext);
        }
    }

    public enum RelationalOperators
    {
        EqualTo,
        NotEqualTo,
        LessThan,
        LessThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo
    }
}
