using Bookings.Application.Abstractions.Services;
using Bookings.Application.DTOs;
using Bookings.Domain.Entities;
using Bookings.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Server.Controllers;

/// <summary>
/// Эндпоинт бронирования
/// </summary>
[Authorize]
[Route("bookings")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Получение информации бронирования по идентификатору
    /// </summary>
    /// <param name="Id">Идентифкатор</param>
    /// <returns>Объектная модель Booking</returns>
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [HttpGet("{Id}")]
    public async Task<ActionResult<Booking>> Get(Guid Id)
    {
        var user = HttpContext.User.GetUser();
        return await bookingService.GetBookingByIdAsync(Id, user, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Добавления планирования события
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="202">Возвращается JSON-структура AddBookingResult с деталями ответа</response>
    /// <response code="404">Событие не найдено</response>
    /// <response code="409">Свободных мест на это мероприятие нет.</response>
    [ProducesResponseType(typeof(AddBookingResult), StatusCodes.Status202Accepted, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict, contentType: "application/problem+json")]
    [HttpPost("{id}")]
    public async Task<ActionResult<AddBookingResult>> CreateBooking(Guid id)
    {
        var user = HttpContext.User.GetUser();
        var result = await bookingService.CreateBookingAsync(id, user, HttpContext.RequestAborted);

        return Accepted($"/bookings/{result?.Id}", result);
    }

    /// <summary>
    /// Отмена бронирования
    /// </summary>
    /// <param name="Id">Идентификатор брони</param>
    /// <response code="204">Успешное обновление</response>
    /// <response code="400">Ошибка запроса</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Cancel(Guid Id)
    {
        var user = HttpContext.User.GetUser();
        await bookingService.Cancel(Id, user, HttpContext.RequestAborted);

        return new NoContentResult();
    }
}
