using EventDomain.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection;
using System.Text.Json.Serialization;

namespace EventServer.Core;

/// <summary>
/// Конфигурация приложения
/// </summary>
public static class Configure
{
    /// <summary>
    /// Базовая конфигурация приложения
    /// </summary>
    /// <param name="services"></param>
    public static void AddBaseConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.Strict | JsonNumberHandling.WriteAsString;
        });


        services.AddSwaggerGen(options =>
        {
            // Путь к XML-файлу с документацией
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Сервер событий",
                Description = "CRUD запросы событий",

            });
        });

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        //services.AddDbContextFactory<AppDbContext>(options => options.UseNpgsql(connectionString));

    }
}
