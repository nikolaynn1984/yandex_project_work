using Event.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDomain.Extentions
{
    internal static class Funcs
    {
        /// <summary>
        /// Получить спискок с пагинацией
        /// </summary>
        /// <param name="events">Список событий</param>
        /// <param name="page">Страница</param>
        /// <param name="pageSize">Количество записей в странице</param>
        /// <returns>Список</returns>
        internal static IEnumerable<Events> Pagination(this IEnumerable<Events> events, int page, int pageSize)
        {
            return events.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Фильтр списка
        /// </summary>
        /// <param name="events">Список событий</param>
        /// <param name="title">Наименование</param>
        /// <param name="from">Дата начала</param>
        /// <param name="to">Дата окончания</param>
        /// <returns>Список</returns>
        internal static IEnumerable<Events> Filter(this IEnumerable<Events> events, string? title, DateTime? from, DateTime? to)
        {

            var result = events;

            if (!string.IsNullOrEmpty(title))
                result = events.Where(s => s.Title == title);

            if (from != null)
                result = events.Where(s => s.StartAt >= from);

            if (to != null)
                result = events.Where(s => s.EndAt <= to);


            return result;
        }
    }
}
