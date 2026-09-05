using Communication.API;
using Confluent.Kafka;
using Events.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Events.Infrastructure.Services;

public class MessageBroker : IMessageBroker
{
    private readonly IProducer<string, string> producer;
    private readonly ILogger<MessageBroker> logger;
    
    
    public MessageBroker(ILogger<MessageBroker> logger, IConfiguration configuration)
    {
        this.logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = GetServerOrThrow(configuration),
            Acks = Acks.All
        };

        this.producer = new ProducerBuilder<string, string>(config).Build();
    }

    private string GetServerOrThrow(IConfiguration configuration)
    {
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        string result = configuration["Kafka:BootstrapServers"];
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        if (string.IsNullOrEmpty(result))
            throw new InvalidOperationException("Не найдены настройки Kafka:BootstrapServer");

        return result;
    }

    public async Task<bool> PublishAsync(string key, string body)
    {
        try
        {
            await this.producer.ProduceAsync(Topic.Event, new Message<string, string> { Key = key, Value = body });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, $"Ошибка при отправке сообщения в топик {Topic.Event}. Ключ: {key}");
            return false;
        }
        

        return true;
    }
}
