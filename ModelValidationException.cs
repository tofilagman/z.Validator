using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace z.Validator
{
    public class ModelValidationException : Exception
    {
        public readonly ICollection<ValidationResult> Results;

        public ModelValidationException(ICollection<ValidationResult> results)
        {
            Results = results;
        }

        public ModelValidationException(string message)
        {
            Results = new ValidationResult[]
            {
                new ValidationResult(message)
            };
        }
    }
}
