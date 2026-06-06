using Event.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventDomain.Extentions
{
    public static class Funcs
    {
        /// <summary>
        /// Получить спискок с пагинацией
        /// </summary>
        /// <param name="events">Список событий</param>
        /// <param name="page">Страница</param>
        /// <param name="pageSize">Количество записей в странице</param>
        /// <returns>Список</returns>
        public static IEnumerable<Events> Pagination(this IEnumerable<Events> events, int page, int pageSize)
        {
            PageValid(page, "page");
            PageValid(pageSize, "pageSize");
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
        public static IEnumerable<Events> Filter(this IEnumerable<Events> events, string? title, DateTime? from, DateTime? to)
        {

            var result = events;

            if (!string.IsNullOrEmpty(title))
                result = result.Where(s => s.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (from != null)
                result = result.Where(s => s.StartAt >= from);


            if (to != null)
            {
                if(from != null && from > to)
                        throw new ValidationException("Дата окончания должна быть после даты начала");

                result = result.Where(s => s.EndAt <= to);
            }
                


            return result;
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
}
