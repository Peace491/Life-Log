using Microsoft.Net.Http.Headers;
using Peace.Lifelog.LLI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Registration of objects for .NET's DI Container */

builder.Services.AddControllers(); // Controllers are executed as a service within Kestral

// Creation of the WebApplication host object
var app = builder.Build(); // Only part needed to execute Web API project

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* Setup of Middleware Pipeline */

app.UseHttpsRedirection();

// Defining a custom middleware AND adding it to Kestral's request pipeline
// Custom middleware to improve security by stopping web server from advertising itself 
app.Use(async (httpContext, next) =>
{

    // No inbound code to be executed
    //
    //
    httpContext.Response.Headers.Server = "";

    // Go to next middleware
    await next(httpContext);

    // Explicitly only wanting code to execite on the way out of pipeline (Response/outbound direction)
    if (httpContext.Response.Headers.ContainsKey(HeaderNames.XPoweredBy))
    {
        httpContext.Response.Headers.Remove(HeaderNames.XPoweredBy);
    }
});


// Defining a custom middleware AND adding it to Kestral's request pipeline
app.Use((httpContext, next) =>
{
    if (httpContext.Request.Method.ToUpper() == nameof(HttpMethod.Options).ToUpper() &&
        httpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
    {
        var allowedMethods = new List<string>()
        {
            HttpMethods.Get,
            HttpMethods.Post,
            HttpMethods.Options,
            HttpMethods.Head
        };

        httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, "*");
        httpContext.Response.Headers.AccessControlAllowMethods = string.Join(",", allowedMethods); // "GET, POST, OPTIONS, HEAD"
        httpContext.Response.Headers.AccessControlAllowHeaders = "*";
        httpContext.Response.Headers.AccessControlMaxAge = TimeSpan.FromHours(2).Seconds.ToString();
    }

    return next(httpContext);
});

app.MapControllers(); // Needed for mapping the routes defined in Controllers

app.Run(); // Only part needed to execute Web API project