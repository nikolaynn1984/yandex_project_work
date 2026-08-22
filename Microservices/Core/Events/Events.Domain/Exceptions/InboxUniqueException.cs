namespace Events.Domain.Exceptions;

public class InboxUniqueException : Exception
{
    public InboxUniqueException() { }

    public InboxUniqueException(string message) : base(message) { }

}
