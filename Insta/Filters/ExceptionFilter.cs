using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Insta.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var result = new ObjectResult(new ErrorModel()
            {
                Success = false,
                Error = context.Exception.Message
            });
            context.Result = result;
        }
    }
}
