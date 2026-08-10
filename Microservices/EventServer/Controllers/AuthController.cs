using Account.Application.Abstractions.Services;
using Account.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EventServer.Controllers;

/// <summary>
/// Управление пользователями
/// </summary>
[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly IUserService userService;

    /// <summary>
    /// Управление пользователями
    /// </summary>
    /// <param name="userService">Сервси пользователей</param>
    public AuthController(IUserService userService)
    {
        this.userService = userService;
    }

    /// <summary>
    /// Регистрация пользователя
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <remarks>
    /// Роли:
    /// 1 = User - пользователь с ограниченными правами. По умолчанию
    /// 2 = Admin - для администратора
    /// </remarks>
    /// <response code="204">Успешная регистрация</response>
    /// <response code="400">Ошибка запроса</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, contentType: "application/problem+json")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await this.userService.Register(request, HttpContext.RequestAborted);

        return new NoContentResult();
    }

    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <response code="200">Успешная авторизация</response>
    /// <response code="400">Ошибка запроса</response>
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK, contentType: "application/json")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest, contentType: "application/problem+json")]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult?>> Login(LoginRequest request)
    {
        return await this.userService.Login(request, HttpContext.RequestAborted);
    }
}
