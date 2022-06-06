using DietAssistant.Common;
using Newtonsoft.Json;
using System.Net;

namespace DietAssistant.WebAPI.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
            => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(
                Result.CreateWithError<object>(EvaluationTypes.Failed, exception.Message));

            context.Response.ContentType = "application/json";

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
