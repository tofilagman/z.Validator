using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace z.Validator
{
    public abstract class ValidatorService
    {
        protected ModelValidationException ModelException { get; private set; }

        public void ValidateToken<T>(T obj)
        {
            if (!IsValid(obj))
                throw ModelException;
        }

        public void ValidateListToken<T>(List<T> obj)
        {
            if (!IsListValid(obj))
                throw ModelException;
        }

        public bool IsValid<T>(T obj)
        {
            var vc = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, vc, results, true))
            {
                ModelException = new ModelValidationException(results);
                return false;
            }
            return true;
        }

        public bool IsListValid<T>(List<T> obj)
        {
            if (obj == null)
                return true;

            foreach (var d in obj)
                if (!IsValid(d))
                    return false;
            return true;
        }
    }
}
