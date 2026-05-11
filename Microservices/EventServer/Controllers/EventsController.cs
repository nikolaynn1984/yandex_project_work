using Event.Domain.Interfaces;
using Event.Domain.Models;
using EventDomain.Extentions;
using EventDomain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;



namespace EventServer.Controllers;

/// <summary>
/// Эендпоинт событий
/// </summary>
/// <param name="eventService"></param>
[Route("events")]
[ApiController]
public class EventsController(IEventService eventService) : ControllerBase
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
    [ProducesResponseType(typeof(PaginatedResult), StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpGet] //("{title?}/{from?}/{to?}/{page?}/{pageSize?}")
    public ActionResult<PaginatedResult> Get(string? title = null, DateTime? from = null, DateTime? to = null, int? page = 1, int? pageSize = 10)
    {
        return eventService.Get(title, from, to, page, pageSize);
    }

    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="200">Возвращается JSON-структура Event с деталями ответа</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(Events), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EventException), StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public ActionResult<Events> Get(int id)
    {
        return eventService.Get(id);
    }

    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Модель Event</param>
    /// <response code="201">Успешное добавление</response>
    [HttpPost]
    public IActionResult Post(EventRequest model)
    {
        int id = eventService.Add(model);
        var result = new AddResult() { Id = id };
        return Created("api/events", result);
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="model">Параметры запроса</param>
    /// <response code="204">Успешное обновление</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(EventException), StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public IActionResult Put(int id, EventRequest model)
    {
        eventService.Update(id, model);

        return new OkResult();
    }

    /// <summary>
    /// Удаление события по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="200">Успешное удаление</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(EventException), StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        eventService.Delete(id);

        return new OkResult();
    }
}
