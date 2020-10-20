using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SP.Messenger.Application.Exceptions;

namespace SP.Messenger.Hub.Service.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, logger, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, ILogger log, Exception exception)
        {
            log.LogError(exception, "An unhandled exception has occurred");
            Serilog.Log.Error(exception, "Serilog error An unhandled exception has occurred");
            
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            string result;

//            switch (exception)
//            {
//                case  SP.Catalog.Application.Exceptions.ValidationException exp:
//                    result = JsonConvert.SerializeObject(new { error = exp.Failures });
//                    break;
//                case AlreadyExistsException _:
//                case DeleteFailureException _:
//                    code = HttpStatusCode.Conflict;
//                    result = JsonConvert.SerializeObject(new { error = exception.Message });
//                    break;
//                default:
//                    result = JsonConvert.SerializeObject(new { error = exception.Message });
//                    break;
//            }

            result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
