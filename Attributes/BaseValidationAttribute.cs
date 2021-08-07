using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using z.Data;

namespace z.Validator.Attributes
{
    public abstract class BaseValidationAttribute : ValidationAttribute
    {
        public string Key { get; set; }
        public object[] PlaceHolderValues { get; set; }

        public ValidationResult CreateValidationErrorResult(ValidationContext validationContext, params string[] otherMembers)
        {
            var memberNames = new List<string>();
            memberNames.Add(validationContext.MemberName.HumanizeColumn());
            memberNames.AddRange(otherMembers.Select(x => x.HumanizeColumn()));

            return new ValidationResult(ErrorMessageString, memberNames);
        }

        protected abstract ValidationResult OnValidate(object value, ValidationContext validationContext);

        protected sealed override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var member = validationContext.ObjectType.GetMember(validationContext.MemberName);
            var ignore = member[0].GetCustomAttribute(typeof(IgnoreAttribute));

            if (ignore == null)
                return OnValidate(value, validationContext);
            else
                return ValidationResult.Success;
        }
    }
}
