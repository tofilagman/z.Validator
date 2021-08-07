using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace z.Validator
{
    /// <summary>
    /// Use ValidatorService to implement validation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IThrowableValidator<T> where T : class
    {
    }
}
