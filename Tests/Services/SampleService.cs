using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Validator.Test.Models;

namespace z.Validator.Test.Services
{
    public class SampleService : ValidatorService, IThrowableValidator<TestThrowableToken>,
        IThrowableValidator<TestAsyncToken>,
        IAutoUniqueValidator<UniqueToken>
    {
        public async Task<TestToken> TestAsync(TestToken token)
        {
            ValidateToken(token);

            return token;
        }

        public async Task<TestThrowableToken> TestThrowableAsync(TestThrowableToken token)
        {
            ValidateToken(token);
            return token;
        }

        public void ValidateLastName(TestThrowableToken token, string name)
        {
            if (token.LastName == null)
                throw new Exception("Lastname is expected");
            if (token.LastName == "Rizal")
                throw new Exception("Lastname should be Penduko");
        }

        public async Task<TestAsyncToken> TestAsyncronousValidation(TestAsyncToken token)
        {
            ValidateToken(token);
            return token;
        }

        public async Task ValidateAsyncLastName(TestAsyncToken token, string name)
        {
            if (token.LastName == null)
                throw new Exception("Lastname is expected");
            if (token.LastName == "Rizal")
                throw new Exception("Lastname should be Penduko");

            await Task.CompletedTask;
        }

        public async Task<UniqueToken> TestUnique(UniqueToken token)
        {
            ValidateToken(token);
            return token;
        }
         
        public async Task<bool> IsUnique(UniqueToken obj, string member)
        {
            return await Task.FromResult(obj.LastName != null);
        }
    }
}
