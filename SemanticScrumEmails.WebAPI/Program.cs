using System.Diagnostics;
using SemanticScrumEmails.interfaces;
using SemanticScrumEmails.Queries.DevOps;
using SemanticScrumEmails.services;
using SemanticScrumEmails.WebAPI.endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register the Swagger services
builder.Services.AddSwaggerDocument(config =>
{
    config.PostProcess = document =>
    {
        document.Info.Version = "v1";
        document.Info.Title = "My API";
        document.Info.Description = "A simple ASP.NET Core Web API";
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();


builder.Services.AddScoped<IDevOpsService, DevOpsService>();
builder.Services.AddScoped<GetSprintDetailsService>();
builder.Services.AddScoped<GetAssignedTasksService>();

var app = builder.Build();

app.MapDevOpsEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseCors(builder =>
    builder.AllowAnyOrigin() // adjust the origin accordingly
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

if (app.Environment.IsDevelopment())
{
    var url = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
    Process.Start(new ProcessStartInfo
    {
        FileName = url + "/swagger",
        UseShellExecute = true
    });
}