using System.Net.Http; // For HttpMethod

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS implementation
app.Use((httpContext, next) => 
{
    // Address Browser's Preflight OPTIONS request
    if(httpContext.Request.Method == nameof(HttpMethod.Options).ToUpperInvariant())
    {
        httpContext.Response.StatusCode = 204; 
        httpContext.Response.Headers.AccessControlAllowOrigin = "http://localhost:3000/";
        httpContext.Response.Headers.AccessControlAllowMethods = "GET,POST,OPTIONS,PUT,DELETE";
        httpContext.Response.Headers.AccessControlAllowHeaders = "*";
        httpContext.Response.Headers.AccessControlAllowCredentials = "true";
        

        return Task.CompletedTask; // Terminate the HTTP Request
    }

    return next();
});

app.Use((httpContext, next) => 
{
    httpContext.Response.Headers.AccessControlAllowOrigin = "http://localhost:3000/"; 
    httpContext.Response.Headers.AccessControlAllowMethods = "GET,POST,OPTIONS,PUT,DELETE";
    httpContext.Response.Headers.AccessControlAllowHeaders = "*";
    httpContext.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});



app.MapControllers();

app.Run();