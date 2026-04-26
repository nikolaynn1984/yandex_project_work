using EventServer.Core;
using System.ComponentModel.DataAnnotations;

namespace EventServer.Models
{
    /// <summary>
    /// Запрос обновления события
    /// </summary>
    public class EventRequest
    {
        /// <summary>
        /// Титл
        /// </summary>
        [Required(ErrorMessage = "Свойство Title обязательно для заполнения")]
        public string Title { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Начало
        /// </summary>
        [Required(ErrorMessage = "Свойство StartAt обязательно для заполнения")]
        [DataType(DataType.DateTime)]
        [DateLessThan("EndAt", ErrorMessage = "Дата окончания должна быть после даты начала")]
        public DateTime? StartAt { get; set; }
        /// <summary>
        /// Конец
        /// </summary>
        [Required(ErrorMessage = "Свойство EndAt обязательно для заполнения")]
        [DataType(DataType.DateTime)]
        public DateTime? EndAt { get; set; }
    }
}
