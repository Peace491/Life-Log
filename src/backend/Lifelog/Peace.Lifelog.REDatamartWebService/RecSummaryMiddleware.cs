namespace Peace.Lifelog.RecSummaryWebService;
using Microsoft.Net.Http.Headers;

public class RecSummaryMiddleware
{
 private readonly LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();
    private readonly RequestDelegate _next;

    public RecSummaryMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {

        if (httpContext.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
        {
            var allowedMethods = new List<string>()
                  {
            HttpMethods.Get,
            HttpMethods.Post,
            HttpMethods.Options,
            HttpMethods.Head,
            HttpMethods.Delete
                  };

            httpContext.Response.StatusCode = 204;

            httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, lifelogConfig.HostURL);
            httpContext.Response.Headers.AccessControlAllowMethods = string.Join(", ", allowedMethods);
            httpContext.Response.Headers.AccessControlAllowHeaders = "*";
            httpContext.Response.Headers.AccessControlAllowCredentials = "true";

            await _next(httpContext);
        }
    }
}

public static class CustomRecSummaryMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomRecSummaryMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RecSummaryMiddleware>();
    }
}