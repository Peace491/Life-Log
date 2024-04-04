using Microsoft.Net.Http.Headers;


namespace Peace.Lifelog.RecSummaryWebService;

public class LifelogCorsMiddleware
{
    private readonly LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();
    private readonly RequestDelegate _next;

    public LifelogCorsMiddleware(RequestDelegate next)
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

public static class CustomCorsMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomCorsMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LifelogCorsMiddleware>();
    }
}
