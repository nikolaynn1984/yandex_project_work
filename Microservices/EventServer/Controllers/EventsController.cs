using EventDomain.Interfaces;
using EventDomain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventServer.Controllers;

/// <summary>
/// Эендпоинт событий
/// </summary>
/// <param name="eventService">Сервис событий</param>
/// <param name="bookingService">Сервис бронирований</param>
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
    [ProducesResponseType(typeof(PaginatedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [Produces("application/json")]
    [HttpGet] //("{title?}/{from?}/{to?}/{page?}/{pageSize?}")
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
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
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
    [ProducesResponseType(typeof(AddBookingResult), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [Produces("application/json")]
    [HttpPost("{id}/book")]
    public async Task<ActionResult<AddBookingResult>> CreateBooking(Guid id)
    {
        var result = await bookingService.CreateBookingAsync(id, HttpContext.RequestAborted);

        return Accepted($"/bookings/{result?.Id}", result);
    }

    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Модель Event</param>
    /// <response code="201">Успешное добавление</response>
    /// <response code="400">Ошибка запроса</response>
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post(EventRequest model)
    {
        Guid id =  await eventService.Add(model, HttpContext.RequestAborted);
        var result = new AddResult() { Id = id };
        return Created(HttpContext.Request.Path, result);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="model">Параметры запроса</param>
    /// <response code="204">Успешное обновление</response>
    /// <response code="400">Ошибка запроса</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, EventRequest model)
    {
        await eventService.Update(id, model, HttpContext.RequestAborted);

        return new OkResult();
    }

    /// <summary>
    /// Удаление события по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="200">Успешное удаление</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await eventService.Delete(id, HttpContext.RequestAborted);

        return new OkResult();
    }
}
