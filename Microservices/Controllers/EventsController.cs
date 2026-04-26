using EventServer.Core.Interfaces;
using EventServer.Models;
using Microsoft.AspNetCore.Mvc;



namespace EventServer.Controllers;

/// <summary>
/// Эендпоинт событий
/// </summary>
/// <param name="eventService"></param>
[Route("api/[controller]")]
[ApiController]
public class EventsController(IEventService eventService) : ControllerBase
{
    /// <summary>
    /// Получить список событий
    /// </summary>
    /// <response code="200">Список событие</response>
    [ProducesResponseType(typeof(List<Event>), StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpGet]
    public ActionResult<IEnumerable<Event>> Get()
    {
        return eventService.Get();
    }

    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <response code="200">Возвращается JSON-структура Event с деталями ответа</response>
    /// <response code="404">Событие не найдено</response>
    [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
    [Produces("application/json")]
    [HttpGet("{id}")]
    public ActionResult<Event> Get(int id)
    {
        var result = eventService.Get(id);

        if (result == null) return new NotFoundResult();


        return result;
    }

    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Модель Event</param>
    /// <response code="201">Успешное добавление</response>
    [HttpPost]
    public IActionResult Post(Event model)
    {
        eventService.Add(model);
        return new CreatedResult();
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <param name="model">Параметры запроса</param>
    /// <response code="204">Успешное обновление</response>
    /// <response code="404">Событие не найдено</response>
    [HttpPut("{id}")]
    public IActionResult Put(int id, UpdateRequest model)
    {
        bool result =  eventService.Update(id, model);

        if (result == false) return new NotFoundResult(); 

        return new NoContentResult();
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
        bool result =  eventService.Delete(id);

        if(result == false) return new NotFoundResult();

        return new OkResult();
    }
}
