using Microsoft.Net.Http.Headers;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Security;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Map;
using Peace.Lifelog.PinService;
using Peace.Lifelog.LocationRecommendationService;

var builder = WebApplication.CreateBuilder(args);

LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();


/* Registration of objects for .NET's DI Container */
builder.Services.AddTransient<IReadDataOnlyDAO, ReadDataOnlyDAO>();
builder.Services.AddTransient<ICreateDataOnlyDAO, CreateDataOnlyDAO>();
builder.Services.AddTransient<IDeleteDataOnlyDAO, DeleteDataOnlyDAO>();
builder.Services.AddTransient<IUpdateDataOnlyDAO, UpdateDataOnlyDAO>();
builder.Services.AddTransient<ILogTarget, LogTarget>();
builder.Services.AddTransient<ILogging, Logging>();
builder.Services.AddTransient<IMapRepo, MapRepo>(); 
builder.Services.AddTransient<IPinService, PinService>();
builder.Services.AddTransient<ILocationRecommendationService, LocationRecommendationService>();
builder.Services.AddTransient<ILifelogAuthService, LifelogAuthService>();

builder.Services.AddEndpointsApiExplorer();
// for using swagger remove before pushing to main
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(); // Controllers are executed as a service within Kestral

// Creation of the WebApplication host object
var app = builder.Build(); // Only part needed to execute Web API project

if (app.Environment.IsDevelopment()) // make sure to remove
{
    app.UseSwagger();     // make sure to remove
    app.UseSwaggerUI();      // make sure to remove
}

/* Setup of Middleware Pipeline */

// app.UseHttpsRedirection();


// Defining a custom middleware AND adding it to Kestral's request pipeline
app.Use((httpContext, next) =>
{
    if (httpContext.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
    {
        var allowedMethods = new List<string>()
        {
            HttpMethods.Get,
            HttpMethods.Post,
            HttpMethods.Put,
            HttpMethods.Options,
            HttpMethods.Head,
            HttpMethods.Delete
        };

        httpContext.Response.StatusCode = 204;

        httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, lifelogConfig.HostURL);
        httpContext.Response.Headers.AccessControlAllowMethods = string.Join(", ", allowedMethods);
        httpContext.Response.Headers.AccessControlAllowHeaders = "*";
        httpContext.Response.Headers.AccessControlAllowCredentials = "true";

        return Task.CompletedTask; // Terminate Request right away

    }

    return next();
});

app.Use((httpContext, next) =>
{

    var allowedMethods = new List<string>()
    {
        HttpMethods.Get,
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Options,
        HttpMethods.Head,
        HttpMethods.Delete
    };

    httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, lifelogConfig.HostURL);
    httpContext.Response.Headers.AccessControlAllowMethods = string.Join(", ", allowedMethods);
    httpContext.Response.Headers.AccessControlAllowHeaders = "*";
    httpContext.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});


app.MapControllers(); // Needed for mapping the routes defined in Controllers


app.Run(); // Only part needed to execute Web API project