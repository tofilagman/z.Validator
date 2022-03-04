using System;
using System.ComponentModel.DataAnnotations;

namespace z.Validator.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class LengthAttribute : BaseValidationAttribute
    {
        public int Size { get; set; }
        public RelationalOperators Operator { get; set; }

        public LengthAttribute(int size, RelationalOperators @operator)
        {
            Size = size;
            Operator = @operator;
        }

        protected override ValidationResult OnValidate(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            // finally compare the two values
            var operand1 = (IComparable)value.ToString().Length;
            var operand2 = (IComparable)Size;
            int comparison = operand1.CompareTo(operand2);

            var isValid =
                comparison < 0 &&
                    (Operator == RelationalOperators.LessThan || Operator == RelationalOperators.LessThanOrEqualTo || Operator == RelationalOperators.NotEqualTo) ||
                comparison > 0 &&
                    (Operator == RelationalOperators.GreaterThan || Operator == RelationalOperators.GreaterThanOrEqualTo || Operator == RelationalOperators.NotEqualTo) ||
                comparison == 0 &&
                    (Operator == RelationalOperators.EqualTo || Operator == RelationalOperators.GreaterThanOrEqualTo || Operator == RelationalOperators.LessThanOrEqualTo);

            if (isValid)
                return ValidationResult.Success;

            return CreateValidationErrorResult(validationContext);
        }


    }
}
