using EventApplication.Abstractions.Services;
using EventApplication.Bookings.DTOs;
using EventApplication.Events.DTOs;
using EventDomain.Entities;
using EventInfrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventServer.Controllers;

/// <summary>
/// Эендпоинт событий
/// </summary>
/// <param name="eventService">Сервис событий</param>
/// <param name="bookingService">Сервис бронирований</param>
[Authorize]
[Route("events")]
[ApiController]
public class EventsController(IEventService eventService, IBookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Получить список событий
    /// </summary>
    /// <param name="title">поиск по названию (не обязательный параметр) </param>
    /// <param name="from">поиск по дате начала события (не обязательный параметр) формат - 2026-05-11T11:41:33.182Z</param>
    /// <param name="to">посик по дате окончания события (не обязательный параметр) формат - 2026-05-11T11:41:33.182Z</param>
    /// <param name="page">Страница которую требуется вернуть, по умолчанию 1</param>
    /// <param name="pageSize">Количество элементов в странице, по умолчанию 10</param>
    /// <response code="200">Список событие</response>
    /// <response code="400">Ошибка запроса</response>
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult), StatusCodes.Status200OK, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, contentType: "application/problem+json")]
    [HttpGet]
    public async Task<ActionResult<PaginatedResult>> Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
    {
        return await eventService.Get(title, from, to, page, pageSize, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="200">Возвращается JSON-структура Event с деталями ответа</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Event>> Get(Guid id)
    {
        return await eventService.GetAsync(id, HttpContext.RequestAborted);
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
    [HttpPost("{id}/book")]
    public async Task<ActionResult<AddBookingResult>> CreateBooking(Guid id)
    {
        var user = HttpContext.User.GetUser();
        var result = await bookingService.CreateBookingAsync(id, user, HttpContext.RequestAborted);

        return Accepted($"/bookings/{result?.Id}", result);
    }

    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Модель Event</param>
    /// <response code="201">Успешное добавление</response>
    /// <response code="400">Ошибка запроса</response>
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AddResult), StatusCodes.Status201Created, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, contentType: "application/problem+json")]
    [HttpPost]
    public async Task<IActionResult> Post(EventRequest model)
    {
        Guid id =  await eventService.Add(model, HttpContext.RequestAborted);
        var result = new AddResult() { Id = id };
        return Created($"/events/{id}", result);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="model">Параметры запроса</param>
    /// <response code="204">Успешное обновление</response>
    /// <response code="400">Ошибка запроса</response>
    /// <response code="404">Событие не найдено</response>
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, contentType: "application/problem+json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound, contentType: "application/problem+json")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, EventRequest model)
    {
        await eventService.Update(id, model, HttpContext.RequestAborted);

        return new NoContentResult();
    }

    /// <summary>
    /// Удаление события по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="204">Успешное удаление</response>
    /// <response code="404">Событие не найдено</response>
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await eventService.Delete(id, HttpContext.RequestAborted);

        return new NoContentResult();
    }
}
