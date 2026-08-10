using Account.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Account.Application.DTOs;
/// <summary>
/// Параметры запроса регистрации
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Поле Login обязательно для заполнения")]
    public required string Login { get; set; }
    [Required(ErrorMessage = "Поле Password обязательно для заполнения")]
    public required string Password { get; set; }
    public RoleType Role { get; set; } = RoleType.User;
}
