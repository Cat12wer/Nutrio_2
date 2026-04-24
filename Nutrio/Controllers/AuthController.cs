using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrio.Application.Commands.Auth.Google;
using Nutrio.Application.Commands.Auth.Login;
using Nutrio.Application.Commands.Auth.Register;

namespace Nutrio.Controllers;

[ApiController] // Каже ASP.NET, що це API-контролер (автоматично перевіряє базові помилки формату)
[Route("api/[controller]")] // Шлях буде: /api/auth
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    // Впроваджуємо MediatR через конструктор
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Ендпоінт: POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        // MediatR сам знайде RegisterUserCommandHandler, виконає його і поверне AuthResultDto
        var result = await _mediator.Send(command);
        return Ok(result); // Повертаємо HTTP 200 OK з даними
    }

    // Ендпоінт: POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Ендпоінт: POST /api/auth/google
    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthDTO command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}