using Peace.Lifelog.UserManagement;
using Peace.Lifelog.UserManagementTest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapGet("/getProfile", () =>
{
    var testLifelogProfile = new LifelogProfile();

    testLifelogProfile.UserId = "TestUserHash";
    testLifelogProfile.DOB = DateTime.Now.ToString("yyyy-MM-dd");
    testLifelogProfile.ZipCode = "92612";

    var list = new List<LifelogProfile>(){testLifelogProfile};

    
    return list;
})
.WithName("GetProfile")
.WithOpenApi();

app.Run();