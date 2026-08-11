namespace EventDomain.Exceptions;

public class ForbiddenExeption : Exception
{
    public ForbiddenExeption() { }

    public ForbiddenExeption(string message) : base(message) {
}
