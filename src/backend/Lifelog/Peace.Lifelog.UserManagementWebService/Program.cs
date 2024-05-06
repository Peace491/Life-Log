using Microsoft.Net.Http.Headers;
using Peace.Lifelog.DataAccess;
using Peace.Lifelog.Infrastructure;
using Peace.Lifelog.Logging;
using Peace.Lifelog.Security;
using Peace.Lifelog.UserForm;
using Peace.Lifelog.UserManagementWebService.Controllers;
using Peace.Lifelog.LifelogReminder;
using Peace.Lifelog.UserManagement;

var builder = WebApplication.CreateBuilder(args);

/* Registration of objects for .NET's DI Container */

builder.Services.AddControllers(); // Controllers are executed as a service within Kestral

// Dependency Injection
builder.Services.AddTransient<ICreateDataOnlyDAO, CreateDataOnlyDAO>();
builder.Services.AddTransient<CreateDataOnlyDAO, CreateDataOnlyDAO>();
builder.Services.AddTransient<IReadDataOnlyDAO, ReadDataOnlyDAO>();
builder.Services.AddTransient<IUpdateDataOnlyDAO, UpdateDataOnlyDAO>();
builder.Services.AddTransient<IDeleteDataOnlyDAO, DeleteDataOnlyDAO>();
builder.Services.AddTransient<IUserFormRepo, UserFormRepo>();
builder.Services.AddTransient<ILifelogAuthService, LifelogAuthService>();
builder.Services.AddTransient<ILogTarget, LogTarget>();
builder.Services.AddTransient<ILogging, Logging>();
builder.Services.AddTransient<IJWTService, JWTService>();
builder.Services.AddTransient<IUserFormService, UserFormService>();
builder.Services.AddTransient<ILifelogReminderService, LifelogReminderService>();
builder.Services.AddTransient<ILifelogReminderRepo, LifelogReminderRepo>();
builder.Services.AddTransient<AppUserManagementService, AppUserManagementService>();
builder.Services.AddTransient<ILifelogUserManagementService, LifelogUserManagementService>();
builder.Services.AddTransient<SaltService, SaltService>();
builder.Services.AddTransient<IUserManagmentRepo, UserManagmentRepo>();


var config = LifelogConfig.LoadConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Creation of the WebApplication host object
var app = builder.Build(); // Only part needed to execute Web API project

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* Setup of Middleware Pipeline */
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

        httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, config.HostURL);
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
        HttpMethods.Put,
        HttpMethods.Options,
        HttpMethods.Head,
        HttpMethods.Delete
    };

    httpContext.Response.Headers.Append(HeaderNames.AccessControlAllowOrigin, config.HostURL);
    httpContext.Response.Headers.AccessControlAllowMethods = string.Join(", ", allowedMethods);
    httpContext.Response.Headers.AccessControlAllowHeaders = "*";
    httpContext.Response.Headers.AccessControlAllowCredentials = "true";

    return next();
});


app.MapControllers(); // Needed for mapping the routes defined in Controllers


app.Run(); // Only part needed to execute Web API project