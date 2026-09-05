namespace Events.Application.Abstractions.Services;

public interface IMessageBroker
{
    Task<bool> PublishAsync(string Type, string body);
}
