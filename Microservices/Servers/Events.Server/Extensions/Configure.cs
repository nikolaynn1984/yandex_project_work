using Events.Infrastructure.DataAccess;
using Events.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace Events.Server.Extensions;

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

        services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.Strict | JsonNumberHandling.WriteAsString;
        });
        var optionsJwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

        services.AddLogging(logging =>
        {
            logging.AddSimpleConsole(option =>
            {
                option.TimestampFormat = "dd.MM.yyyy HH:mm:ss.fff ";
                option.SingleLine = true;
                option.IncludeScopes = false;
            });
        });

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

        services.AddDbContext<EventDbContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Events.Server")));


        


        

        //try
        //{
        //    services.AddSingleton<IConnectionMultiplexer>(
        //     ConnectionMultiplexer.Connect(redisConnection)
        //    );
        //}
        //catch (Exception)
        //{
        //    services.AddSingleton<IConnectionMultiplexer>(
        //     ConnectionMultiplexer.Connect("")
        //);
        //}


    }

    public static void AddRedis(this IServiceCollection services, WebApplicationBuilder builder)
    {

        var redisConnection = builder.Configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Connection string 'Redis" +
            "' not found.");

        try
        {
            var redis = ConnectionMultiplexer.Connect(redisConnection);
            var db = redis.GetDatabase();
            db.Ping();
            Console.WriteLine("Redis доступен");


            services.AddSingleton<IConnectionMultiplexer>(
             ConnectionMultiplexer.Connect(redisConnection)
            );
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Не удалось подключиться к Redis: {ex.Message}");

            services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexerEmpty>();
        }
    }
}
