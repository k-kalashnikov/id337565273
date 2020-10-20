using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SP.Market.Core.HttpResponses.Responses;
using SP.Messenger.Common.Extensions;

namespace SP.Messenger.Hub.Service.Filters
{
    public class ApiBehaviorInvalidModelResponse
    {
        public static IActionResult Response(ActionContext context)
        {
            var errors = context.ModelState
                .Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                .ToArray();

            var count = errors.Count();
            var arrayInvalidParams = new string[count];
            for (int i = 0; i < count; i++)
                arrayInvalidParams[i] = errors[i].Key;

            var invalidParamsString = string.Join(",", arrayInvalidParams);

            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Instance = context.HttpContext.Request.Path,
                Status = 422,
                Type = "validation-error",
                Detail = $"Please refer to the errors property for additional details: {invalidParamsString}"
            };

            return new JsonResult(HttpCustomResponse.BadRequest(problemDetails.ToJson()))
            {
                ContentType = "application/problem+json"
            };
        }
    }
}