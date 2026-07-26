using EventApplication.Abstractions.Services;
using EventDomain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventServer.Controllers;

/// <summary>
/// Эндпоинт бронирования
/// </summary>
[Route("bookings")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{

    /// <summary>
    /// Получение информации бронирования по идентификатору
    /// </summary>
    /// <param name="Id">Идентифкатор</param>
    /// <returns>Объектная модель Booking</returns>
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

    [Produces("application/json")]
    [HttpGet("{Id}")]
    public async Task<ActionResult<Booking>> Get(Guid Id)
    {
        return await bookingService.GetBookingByIdAsync(Id, HttpContext.RequestAborted);
    }
}
