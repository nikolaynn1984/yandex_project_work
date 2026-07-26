using System.ComponentModel.DataAnnotations;

namespace EventApplication.Events.Extentions;

/// <summary>
/// Валидация даты меньше чем
/// </summary>
internal class DateLessThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="comparisonProperty">Параметр</param>
    public DateLessThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    /// <summary>
    /// Валидация
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="validationContext">Контекст</param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return new ValidationResult("Свойство не должно быть пустым");

        var currentValue = (DateTime)value;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
        if (property == null)
            return new ValidationResult($"Неизвестное свойство: {_comparisonProperty}");

        var comparisonValue = property.GetValue(validationContext.ObjectInstance);
        if (comparisonValue == null) return new ValidationResult("Свойство не должно быть пустым");

        // Дата начала (currentValue) не может быть больше даты окончания (comparisonValue)
        if (currentValue > (DateTime)comparisonValue)
        {
            return new ValidationResult(ErrorMessage ?? "Дата и время начала не может быть больше даты окончания.");
        }

        return ValidationResult.Success;
    }
}
