namespace Account.Domain;

/// <summary>Пользователь</summary>
public class User
{
    /// <summary>Идентификатор</summary>
    public required Guid Id { get; set; }
    /// <summary>Логин пользователя</summary>
    public required string Login { get; set; }
    /// <summary>Хэш пароля</summary>
    public required string PasswordHash { get; set; }
    /// <summary>Роль</summary>
    public RoleType Role { get; set; }
}
