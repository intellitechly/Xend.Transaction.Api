using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xend.Transaction.Api.MiddleWare
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception is DuplicateTransactionException duplicateTransactionException)
            {
                var errorResponse = new ErrorResponse
                {
                    Message = "Duplicate transaction detected.",
                    TransactionId = duplicateTransactionException.TransactionId,
                    // You can include additional error details specific to duplicate transactions
                };

                var errorJson = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(errorJson);
            }
            else
            {
                await HandleOtherExceptionAsync(context, exception);
            }
        }

        private static async Task HandleOtherExceptionAsync(HttpContext context, Exception exception)
        {
            var errorResponse = new ErrorResponse
            {
                Message = "An error occurred while processing your request.",
                // You can include additional error details for other exceptions
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(errorJson);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public int TransactionId { get; set; }
    }
    public class DuplicateTransactionException : Exception
    {
        public int TransactionId { get; }

        public DuplicateTransactionException(int transactionId)
        {
            TransactionId = transactionId;
        }
    }
}
