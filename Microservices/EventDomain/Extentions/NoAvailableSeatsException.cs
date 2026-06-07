namespace EventDomain.Extentions;

internal class NoAvailableSeatsException : Exception
{
    public NoAvailableSeatsException() { }

    public NoAvailableSeatsException(string message) : base(message) { }
}
