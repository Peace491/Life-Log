using Peace.Lifelog.RecSummaryWebService;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

LifelogConfig lifelogConfig = LifelogConfig.LoadConfiguration();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Use((httpContext, next) =>
{
    if(httpContext.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
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

        return Task.CompletedTask; // Terminate Request right away

    }

    return next();
});

app.Use((httpContext, next) => {
    
    var allowedMethods = new List<string>()
    {
        HttpMethods.Get,
        HttpMethods.Post,
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

app.MapControllers();

app.Run();
