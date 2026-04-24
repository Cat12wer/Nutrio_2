using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrio.Application.Queries;
using Nutrio.Application.Queries.Users.GetSidebarSummary;

namespace Nutrio.Controllers;

[Authorize]
[Route("api/[controller]")]
public class SidebarController : BaseController
{
    private readonly IMediator _mediator;

    public SidebarController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Ендпоінт: GET /api/sidebar/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        // Передаємо тільки ID юзера. Запит сам розрахує дані за сьогоднішній день.
        var query = new GetUserSidebarSummaryQuery(GetUserId());
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}