using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shinetech.Common;

namespace API.Filters
{
    public class WebApiResultFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller.GetType().GetCustomAttributes(typeof(FinalResult), true).Length <= 0)
            {
                if (context.Result is ObjectResult)
                {
                    var objectResult = context.Result as ObjectResult;
                    if (objectResult.Value == null)
                    {
                        context.Result = new ObjectResult(new { code = (int)ResponseCode.NotFound, message = "" }) { StatusCode = (int)ResponseCode.NotFound };
                    }
                    else if (objectResult.Value is CommonResponse)
                    {
                        var respon = objectResult.Value as CommonResponse;
                        context.Result = new ObjectResult(new { code = respon.code, message = respon.message, data = respon.data }) { StatusCode = respon.code };
                    }
                    else
                    {
                        context.Result = new ObjectResult(new { code = (int)ResponseCode.OK, message = "", data = objectResult.Value }) { StatusCode = (int)ResponseCode.OK };
                    }
                }
                else if (context.Result is EmptyResult)
                {
                    context.Result = new ObjectResult(new { code = (int)ResponseCode.NotFound, message = "no resource" }) { StatusCode = (int)ResponseCode.NotFound };
                }
                else if (context.Result is ContentResult)
                {
                    context.Result = new ObjectResult(new { code = (int)ResponseCode.OK, message = "", data = (context.Result as ContentResult).Content }) { StatusCode = (int)ResponseCode.OK };
                }
                else if (context.Result is StatusCodeResult)
                {
                    context.Result = new ObjectResult(new { code = (context.Result as StatusCodeResult).StatusCode, message = "" }) { StatusCode = (context.Result as StatusCodeResult).StatusCode };
                }
            }
        }
    }
}
