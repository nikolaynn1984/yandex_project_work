using Events.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Events.Application;

internal static class Funcs
{
    
    internal static SemaphoreSlim bookingLock = new SemaphoreSlim(1, 1);
    /// <summary>
    /// Получить спискок с пагинацией
    /// </summary>
    /// <param name="events">Список событий</param>
    /// <param name="page">Страница</param>
    /// <param name="pageSize">Количество записей в странице</param>
    /// <returns>Список</returns>
    public static IEnumerable<Event> Pagination(this IEnumerable<Event> events, int page, int pageSize)
    {
        PageValid(page, "page");
        PageValid(pageSize, "pageSize");
        return events.Skip((page - 1) * pageSize).Take(pageSize);
    }

    

    private static void PageValid(int value, string name)
    {
        if (value <= 0)
            throw new ValidationException($"Свойство {name} не должно быть меньше 1");
    }


    public static int GetTotalPages(this int totalItems, int pageSize)
    {
        return (int)Math.Ceiling((double)totalItems / pageSize); // Округляем до ближайшего целого вверх
    }
}
