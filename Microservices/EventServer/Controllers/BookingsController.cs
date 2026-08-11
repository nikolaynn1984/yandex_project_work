using EventApplication.Abstractions.Services;
using EventDomain.Entities;
using EventInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [Authorize]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [HttpGet("{Id}")]
    public async Task<ActionResult<Booking>> Get(Guid Id)
    {
        var user = HttpContext.User.GetUser();
        return await bookingService.GetBookingByIdAsync(Id, user, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Отмена бронирования
    /// </summary>
    /// <param name="Id">Идентификатор брони</param>
    /// <response code="204">Успешное обновление</response>
    /// <response code="400">Ошибка запроса</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [HttpDelete("Id")]
    public async Task<IActionResult> Cancel(Guid Id)
    {
        var user = HttpContext.User.GetUser();
        await bookingService.Cancel(Id, user, HttpContext.RequestAborted);

        return new NoContentResult();
    }
}
