using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Xend.Transaction.Api.MiddleWare
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckMiddleware(RequestDelegate next, HealthCheckService healthCheckService)
        {
            _next = next;
            _healthCheckService = healthCheckService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/health"))
            {
                var healthReport = await _healthCheckService.CheckHealthAsync();
                var result = healthReport.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";

                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = healthReport.Status == HealthStatus.Healthy ? 200 : 503;
                await context.Response.WriteAsync(result);
            }
            else
            {
                await _next(context);
            }
        }
    }

    public static class HealthCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthCheckMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<HealthCheckMiddleware>();
            return app;
        }
    }
}
