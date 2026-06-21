namespace EventDomain.Extentions;

public class NoAvailableSeatsException : Exception
{
    public NoAvailableSeatsException() { }

    public NoAvailableSeatsException(string message) : base(message) { }
}
