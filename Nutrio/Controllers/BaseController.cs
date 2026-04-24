using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Nutrio.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    // Цей метод автоматично розшифровує JWT токен і дістає звідти ID користувача
    protected Guid GetUserId()
    {
        // JwtRegisteredClaimNames.Sub під капотом мапиться на ClaimTypes.NameIdentifier
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Користувач не авторизований або токен невалідний");
    }
}