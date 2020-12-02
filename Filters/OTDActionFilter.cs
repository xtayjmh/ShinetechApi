using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using Shinetech.Common;

namespace API.Filters
{
    public class OTDActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new Dictionary<string, string>();
                foreach (var pair in context.ModelState)
                {
                    var errorMessages = pair.Value.Errors.Select(x => x.ErrorMessage);

                    var pairErrors = errorMessages.Aggregate("", (current, error) => current + error + " ");

                    errors.Add(pair.Key, pairErrors);
                }
                context.Result = new JsonResult(new CommonResponse()
                {
                    code = (int)ResponseCode.BadRequest,
                    data = errors,
                })
                { StatusCode = (int)ResponseCode.BadRequest };
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                context.Result = new ObjectResult(new CommonResponse()
                {
                    code = (int)ResponseCode.BadRequest,
                    message = context.Exception.Message,
                    data = context.Exception.Message,
                })
                { StatusCode = (int)ResponseCode.BadRequest };
            }
        }
    }
}
