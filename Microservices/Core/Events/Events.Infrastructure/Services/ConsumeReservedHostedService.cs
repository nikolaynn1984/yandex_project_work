using Communication.API;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Events.Application.Abstractions.Services;
using Events.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Events.Infrastructure.Services;

public class ConsumeReservedHostedService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<ConsumeReservedHostedService> logger;
    private readonly IConsumer<string, string> consumer;
    private readonly string Server;

    public ConsumeReservedHostedService(IServiceScopeFactory scopeFactory, ILogger<ConsumeReservedHostedService> logger, IConfiguration configuration)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
        this.Server = GetConfigOrThrow(configuration, "BootstrapServers");
        var config = new ConsumerConfig
        {
            BootstrapServers = this.Server,
            GroupId = GetConfigOrThrow(configuration, "GroupId"),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false
        };

        this.consumer = new ConsumerBuilder<string, string>(config).Build();


    }

    private string GetConfigOrThrow(IConfiguration configuration, string name)
    {
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        string result = configuration[$"Kafka:{name}"];
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        if (string.IsNullOrEmpty(result))
            throw new InvalidOperationException($"Не найдены настройки Kafka:{name}");

        return result;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Фоновая служба получения бронирований запущена");
        await CheckTopic();

        this.consumer.Subscribe(Topic.Booking);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consume = consumer.Consume(stoppingToken);

                using (var scope = scopeFactory.CreateScope())
                {
                    var reserverdService = scope.ServiceProvider.GetRequiredService<IReservedService>();
                    var message = JsonSerializer.Deserialize<BookingMessage>(consume.Message.Value);
                    if (message != null)
                    {
                        try
                        {
                            await reserverdService.Execute(message.EventId, message.BookingId, message.SeatCount, stoppingToken);

                            consumer.Commit(consume);
                            consumer.StoreOffset(consume);

                            
                        }
                        catch(EventException eex)
                        {
                            consumer.Commit(consume);
                            consumer.StoreOffset(consume);
                            this.logger.LogError(eex.Message);
                        }
                        catch(NoAvailableSeatsException nex)
                        {
                            consumer.Commit(consume);
                            consumer.StoreOffset(consume);
                            this.logger.LogError(nex.Message);
                        }
                        catch (ValidationException vex)
                        {
                            consumer.Commit(consume);
                            consumer.StoreOffset(consume);
                            this.logger.LogError(vex.Message);
                        }
                        catch(DbUpdateException dbex) when (dbex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
                        {
                            consumer.Commit(consume);
                            consumer.StoreOffset(consume);
                        }catch(Exception ex)
                        {
                            this.logger.LogError($"Ошибка обработки сообщения: {ex.Message}");
                        }

                        
                    }

                }


            }
            catch (ConsumeException ex)
            {
                this.logger.LogError($"Ошибка при получении сообщения: {ex.Error.Reason}");
            }
        }
    }


    private async Task CheckTopic()
    {
        try
        {
            IAdminClient admin = new AdminClientBuilder(new AdminClientConfig() { BootstrapServers = this.Server }).Build();
            Metadata metadata = admin.GetMetadata(TimeSpan.FromSeconds(10));

            List<TopicSpecification> addList = new List<TopicSpecification>();
            List<Dictionary<ConfigResource, List<ConfigEntry>>> editList = new List<Dictionary<ConfigResource, List<ConfigEntry>>>();
            bool has = false;
            foreach (var topicSpec in metadata.Topics)
            {
                if (Topic.Booking == topicSpec.Topic)
                {
                    has = true;

                }
            }

            if (has == false)
            {
                var topic = new TopicSpecification()
                {
                    Name = Topic.Booking,
                    Configs = new Dictionary<string, string> { { "retention.ms", "86400000" } }
                };

                await admin.CreateTopicsAsync([topic]);
            }


        }
        catch (Exception ex) { this.logger.LogError(ex, "Не удалось проверить топики брокера сообщений"); }

    }
}
