namespace EventServer.Models
{
    /// <summary>
    /// Запрос обновления события
    /// </summary>
    public class UpdateRequest
    {
        /// <summary>
        /// Титл
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Начало
        /// </summary>
        public DateTime StartAt { get; set; }
        /// <summary>
        /// Конец
        /// </summary>
        public DateTime EndAt { get; set; }
    }
}
