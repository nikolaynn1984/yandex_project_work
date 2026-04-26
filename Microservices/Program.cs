
using EventServer.Core;
using EventServer.Core.Interfaces;
using EventServer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddBaseConfiguration();

builder.Services.AddSingleton<IEventService, EventService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();   
app.UseSwaggerUI(); //swagger/index.html

app.MapControllers();



app.Run();


