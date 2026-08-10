using Account.Application.DTOs;
using EventInfrastructure.DataAccess;
using EventInfrastructure.Services;
using EventServer.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddBaseConfiguration(builder);



builder.Services.AddAccount();

builder.Services.AddEventService();
builder.Services.AddBookingService();

builder.Services.AddExceptions();



var app = builder.Build();


app.UseGlobalExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(); //swagger/index.html

app.MapControllers();



app.Run();