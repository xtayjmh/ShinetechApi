using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Shinetech.Common;
using System;

namespace API.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            Exception exception = context.Exception;
            ObjectResult result = null;
            if (exception is BusinessException)
            {
                result = new ObjectResult(new CommonResponse()
                {
                    code = exception.HResult,
                    message = exception.Message
                })
                { StatusCode = exception.HResult };
            }
            else
            {
                result = new ObjectResult(new CommonResponse()
                {
                    code = (int)ResponseCode.InternalServerError,
                    message = exception.Message
                })
                { StatusCode = (int)ResponseCode.InternalServerError };
                _logger.LogError(exception, "server error");
            }

            context.Result = result;
        }
    }
}
