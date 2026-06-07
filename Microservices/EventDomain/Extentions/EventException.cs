namespace EventDomain.Extentions;

public class EventException : Exception
{
    public EventException() { }

    public EventException(string message) : base(message) { }

}
