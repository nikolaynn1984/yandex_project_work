using Account.Application.DTOs;
using EventInfrastructure.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Security.Claims;
using System.Text;
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
    /// <param name="builder">Строитель приложения</param>
    public static void AddBaseConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
    {

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.Strict | JsonNumberHandling.WriteAsString;
        });
        var optionsJwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();


        if (optionsJwt == null)
            throw new InvalidOperationException("Не найдены настройки Jwt");

        if(optionsJwt.Key.Length < 32)
            throw new InvalidOperationException("Свойство Jwt:Key не может быть меньше 32 знаков");

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,options =>
            {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = optionsJwt?.Issuer,

                    ValidateAudience = true,
                    ValidAudience = optionsJwt?.Audience,

                    //RoleClaimType = ClaimTypes.Role,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(3),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(optionsJwt?.Key))
                };
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
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

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization", // Имя заголовка
                In = ParameterLocation.Header, // Где находится параметр (заголовок)
                Type = SecuritySchemeType.Http, // Тип схемы
                Scheme = "Bearer", // Схема (Bearer)
                BearerFormat = "JWT", // Формат токена
                Description = "Введите JWT-токен для авторизации. Пример: Bearer <ваш_токен>" // Описание
            });

            options.AddSecurityRequirement(document => 
            new OpenApiSecurityRequirement() 
            { 
                [new OpenApiSecuritySchemeReference("Bearer", document)] = [] 
            });
        });


        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("EventServer")));


    }
}
