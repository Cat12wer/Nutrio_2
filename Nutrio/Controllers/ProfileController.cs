using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrio.Application.Commands.Profile.UpdateProfile;
using Nutrio.Application.Commands.Users.CompleteOnboarding;
using Nutrio.Application.Queries.Profile.GetUserProfile;

namespace Nutrio.Controllers;

[Authorize] // Захищаємо ВЕСЬ контролер: сюди неможливо достукатися без JWT токена
[Route("api/[controller]")]
public class ProfileController : BaseController
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Ендпоінт: POST /api/profile/onboarding
    [HttpPost("onboarding")]
    public async Task<IActionResult> CompleteOnboarding([FromBody] CompleteOnboardingDTO dto)
    {
        // Беремо ID з токена, а всі інші дані - з тіла запиту (JSON)
        var command = new CompleteOnboardingCommand(
            GetUserId(),
            dto.Name,
            dto.LastName,
            dto.Sex,
            dto.DateOfBirth,
            dto.Height,
            dto.CurrentWeight,
            dto.TargetWeight,
            dto.WeightGoal,
            dto.ActivityLevel);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // Ендпоінт: GET /api/profile
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        // Дістаємо ID користувача і відправляємо запит у MediatR
        var query = new GetUserProfileQuery(GetUserId());
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    // Ендпоінт: PUT /api/profile
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand commandRequest)
    {
        // Фронтенд надсилає лише вагу і цілі, а ID ми підставляємо самі для безпеки
        var command = new UpdateProfileCommand(
            GetUserId(),
            commandRequest.CurrentWeight,
            commandRequest.TargetWeight,
            commandRequest.WeightGoal,
            commandRequest.ActivityLevel);

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}