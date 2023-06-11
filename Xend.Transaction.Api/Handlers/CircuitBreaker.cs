using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Net;

namespace Xend.Transaction.Api.Handlers
{



    public class CircuitBreakerAttribute : Attribute, IAsyncActionFilter
    {
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public CircuitBreakerAttribute(int exceptionsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(exceptionsAllowedBeforeBreaking, durationOfBreak);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var shouldHandleCircuitBreaker = context.ActionDescriptor.EndpointMetadata
                .Any(em => em.GetType() == typeof(CircuitBreakerAttribute));

            if (!shouldHandleCircuitBreaker)
            {
                await next();
                return;
            }

            try
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    await next();
                });
            }
            catch (BrokenCircuitException)
            {
                // Handle circuit breaker open state (e.g., return a specific HTTP status code)
                context.Result = new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }
        }
    }


}