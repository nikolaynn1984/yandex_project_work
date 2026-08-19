using System.ComponentModel.DataAnnotations;

namespace Account.Application.DTOs;
/// <summary>
/// параметры запроса авторизации
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Поле Login обязательно для заполнения")]
    public required string Login { get; set; }
    [Required(ErrorMessage = "Поле Password обязательно для заполнения")]
    public required string Password { get; set; }
}
