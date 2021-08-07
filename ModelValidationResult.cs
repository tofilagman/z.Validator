using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using z.Data;

namespace z.Validator
{
    public class ModelExceptionResult : StatusCodeResult
    {
        private readonly ModelValidationException Exception;
        private readonly string ErrorCode;
        public ModelExceptionResult(string errorCode, ModelValidationException ex, HttpStatusCode statusCode) : base((int)statusCode)
        {
            this.Exception = ex;
            this.ErrorCode = errorCode;
        }

        public override async void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);

            var strBuilder = new StringBuilder();

            foreach(var st in Exception.Results) 
                strBuilder.AppendLine(string.Format(st.ErrorMessage, st.MemberNames.ToArray()));
             
            var bt = Encoding.UTF8.GetBytes($"{ErrorCode}: {strBuilder}");
            await context.HttpContext.Response.Body.WriteAsync(bt, 0, bt.Length);
        }
    }
}
