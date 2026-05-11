using Event.Domain.Interfaces;
using Event.Domain.Models;
using Microsoft.AspNetCore.Mvc;



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
    /// <response code="200">Список событие</response>
    [ProducesResponseType(typeof(List<Events>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpGet]
    public ActionResult<IEnumerable<Events>> Get()
    {
        return eventService.Get();
    }

    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="200">Возвращается JSON-структура Event с деталями ответа</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(Events), StatusCodes.Status200OK)]
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
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        eventService.Delete(id);

        return new OkResult();
    }
}
