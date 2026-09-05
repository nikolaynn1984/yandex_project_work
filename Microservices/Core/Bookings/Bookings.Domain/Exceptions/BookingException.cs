namespace Bookings.Domain.Exceptions;

public class BookingException : Exception
{
    public BookingException() { }

    public BookingException(string message) : base(message) { }

}

