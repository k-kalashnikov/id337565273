using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using SP.Market.Core.Hosting;
using SP.Market.Core.HttpResponses.Responses;
using SP.Messenger.Application.Exceptions;

namespace SP.Messenger.Hub.Service.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute: ExceptionFilterAttribute
    {
        IHostingEnvironmentService _serviceEnvironment;
        
        public override void OnException(ExceptionContext context)
        {
            _serviceEnvironment = GetService(context, typeof(IHostingEnvironmentService));

            if (context.Exception is ValidationException)
            {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = ResultBadRequest(((ValidationException)context.Exception).Failures);
                return;
            }

            var code = HttpStatusCode.InternalServerError;

            if (context.Exception is NotFoundException)
                code = HttpStatusCode.NotFound;

            if (context.Exception is UnprocessableEntityException)
                code = HttpStatusCode.UnprocessableEntity;

            if (context.Exception is ConflictException)
                code = HttpStatusCode.Conflict;

            if (context.Exception is AuthorizationException)
                code = HttpStatusCode.Forbidden;
            if (context.Exception is WriteFileException)
                code = HttpStatusCode.InternalServerError;

            Log.Fatal($"CustomExceptionFilterAttribute. code:{(int)code}; Exception: {context.Exception}");
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)code;
            context.Result = ResultException(context.Exception.Message, context.Exception.StackTrace, code);
        }
        
        #region private
        private IHostingEnvironmentService GetService(ExceptionContext context, Type service)
        {
            try
            {
                return (IHostingEnvironmentService)context.HttpContext.RequestServices.GetService(service);
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.ToString());
            }
        }

        private IActionResult ResultBadRequest(IDictionary<string, string[]> exceptions)
        {
            var responseErrors = new List<string>();

            foreach (var exp in exceptions)
            {
                foreach (var value in exp.Value)
                    responseErrors.Add(value);
            }

            return new JsonResult(HttpCustomResponse.BadRequest(string.Join("," , responseErrors)));
        }

        private IActionResult ResultException(string message, string stackTrace, HttpStatusCode code)
        {
            var responseErrors = new List<string>();

            responseErrors.Add(message);

            if (!_serviceEnvironment?.GetEnvironment() ?? false)
                responseErrors.Add(stackTrace);

            return new JsonResult(HttpCustomResponse.BadRequest(string.Join("," , responseErrors)));
        }
        #endregion
    }
}