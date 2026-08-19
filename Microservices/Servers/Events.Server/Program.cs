using Events.Infrastructure.DataAccess;
using Events.Infrastructure.Services;
using Events.Server.Extensions;
using Exceptions.Handling;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBaseConfiguration(builder);

builder.Services.AddOpenApi();

builder.Services.AddEventService();

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
    var db = scope.ServiceProvider.GetRequiredService<EventDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(); //swagger/index.html

app.MapControllers();

app.Run();

