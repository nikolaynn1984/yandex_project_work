using Event.Domain.Extentions;
using EventServer.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddBaseConfiguration();

builder.Services.AddEventService();
builder.Services.AddBookingService();

var app = builder.Build();


app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseSwagger();
app.UseSwaggerUI(); //swagger/index.html

app.MapControllers();



app.Run();