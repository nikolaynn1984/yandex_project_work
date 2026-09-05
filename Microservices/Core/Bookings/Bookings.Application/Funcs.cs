namespace Bookings.Application;

internal static class Funcs
{
    
    internal static SemaphoreSlim bookingLock = new SemaphoreSlim(1, 1);

   
}
