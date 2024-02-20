using Peace.Lifelog.LLI;

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

app.MapPost("/postLLI", async (string userHash, string title, string category, string description, string status, string visibility, string deadline, int cost, string recurrenceStatus, string recurrenceFrequency) => {
    var lliService = new LLIService();

    var lli = new LLI();
    lli.Title = title;
    lli.Category = category;
    lli.Description = description;
    lli.Status = status;
    lli.Visibility = visibility;
    lli.Deadline = deadline;
    lli.Cost = cost;
    lli.Recurrence.Status = recurrenceStatus;
    lli.Recurrence.Frequency = recurrenceFrequency;

    var response = await lliService.CreateLLI(userHash, lli);

    return response;

})
.WithName("CreateLLIForUser")
.WithOpenApi();

app.MapGet("/getAllLLI", async (string userHash) =>
{
    var lliService = new LLIService();
    var response = await lliService.GetAllLLIFromUser(userHash);

    return response.Output;        
})
.WithName("GetAllLLIForUser")
.WithOpenApi();

app.MapPut("/putLLI", async (string userHash, string lliId, string title = "", string category = "", 
string description = "", string status = "", string visibility = "", string deadline = "", 
int? cost = null, string recurrenceStatus = "", string recurrenceFrequency = "") => {
    var lliService = new LLIService();

    var newLLI = new LLI();
    newLLI.LLIID = lliId;
    newLLI.Title = title;
    newLLI.Category = category;
    newLLI.Description = description;
    newLLI.Status = status;
    newLLI.Visibility = visibility;
    newLLI.Deadline = deadline;
    newLLI.Cost = cost;
    newLLI.Recurrence.Status = recurrenceStatus;
    newLLI.Recurrence.Frequency = recurrenceFrequency;

    var response = await lliService.UpdateLLI(userHash, newLLI);

    return response;

})
.WithName("UpdateLLIForUser")
.WithOpenApi();

app.MapDelete("/deleteLLI", async (string userHash, string lliId) => {
    var lliService = new LLIService();
    var lli = new LLI();
    lli.LLIID = lliId;

    var response = await lliService.DeleteLLI(userHash, lli);

    return response;
})
.WithName("DeleteLLIForUser")
.WithOpenApi();

app.Run();
