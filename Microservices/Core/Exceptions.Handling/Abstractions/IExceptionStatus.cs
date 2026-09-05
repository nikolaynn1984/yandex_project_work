using Microsoft.AspNetCore.Mvc;
using System;

namespace Exceptions.Handling.Abstractions;

/// <summary>
/// Статус исключения
/// </summary>
public interface IExceptionStatus
{
    /// <summary>
    /// Тип исключения
    /// </summary>
    Type Type { get; }
    /// <summary>
    /// Преоброзование исключения
    /// </summary>
    /// <param name="ex">Исключение</param>
    /// <returns>ProblemDetails</returns>
    ProblemDetails Map(Exception ex);
}
