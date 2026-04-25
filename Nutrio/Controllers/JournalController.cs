using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// Виправлені using'и: Add та Delete тепер правильно тягнуться з Commands, а не з Queries!
using Nutrio.Application.Commands.Journal.AddFoodEntry;
using Nutrio.Application.Commands.Journal.DeleteFoodEntry;
using Nutrio.Application.Commands.Journal.UpdateFoodEntry;
using Nutrio.Application.Queries.Journal.Activity;
using Nutrio.Application.Queries.Journal.Meals;
using Nutrio.Application.Queries.Journal.Nutrients;
using Nutrio.Application.Queries.Journal.Products;
namespace Nutrio.Controllers;

[Authorize] // Захищаємо контролер, доступ тільки з JWT токеном
[Route("api/[controller]")]
public class JournalController : BaseController
{
    private readonly IMediator _mediator;

    public JournalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. ПОШУК: GET /api/journal/products/search?term=Банан
    [HttpGet("products/search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string term)
    {
        var query = new SearchProductsQuery(term);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 2. НУТРІЄНТИ (КБЖВК картки): GET /api/journal/nutrients?date=2026-04-23
    [HttpGet("nutrients")]
    public async Task<IActionResult> GetDailyNutrients([FromQuery] DateTime date)
    {
        var query = new GetDailyNutrientsQuery(GetUserId(), date);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 3. СПИСОК ЇЖІ ЗА ДЕНЬ: GET /api/journal/entries?date=2026-04-23
    [HttpGet("entries")]
    public async Task<IActionResult> GetDailyFoodEntries([FromQuery] DateTime date)
    {
        var query = new GetDailyFoodEntriesQuery(GetUserId(), date);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 4. ДОДАТИ ЇЖУ: POST /api/journal/entries
    [HttpPost("entries")]
    public async Task<IActionResult> AddFoodEntry([FromBody] AddFoodEntryCommand requestCommand)
    {
        // Фронтенд не повинен надсилати UserId, ми беремо його безпечно з токена
        var command = new AddFoodEntryCommand(
            GetUserId(),
            requestCommand.ProductId,
            requestCommand.Date,
            requestCommand.MealType,
            requestCommand.Grams);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // 5. ОНОВИТИ ВАГУ (ГРАМИ) ЇЖІ: PUT /api/journal/entries/{id}
    [HttpPut("entries/{id:guid}")]
    public async Task<IActionResult> UpdateFoodEntry(Guid id, [FromBody] UpdateFoodEntryRequest request)
    {
        // Передаємо UserId для перевірки, щоб ніхто не міг змінити чужий запис
        var command = new UpdateFoodEntryCommand(GetUserId(), id, request.Grams);

        // Просто чекаємо виконання (БЕЗ var result = ...)
        await _mediator.Send(command);

        // Повертаємо 200 OK без тіла відповіді (БЕЗ result)
        return Ok();
    }

    // 6. ВИДАЛИТИ ЇЖУ: DELETE /api/journal/entries/{id}
    [HttpDelete("entries/{id:guid}")]
    public async Task<IActionResult> DeleteFoodEntry(Guid id)
    {
        // Знову передаємо UserId для безпеки
        var command = new DeleteFoodEntryCommand(GetUserId(), id);
        await _mediator.Send(command);

        return NoContent(); // 204 NoContent - успішно видалено, повертати нічого
    }

    // 7. СЕРІЯ ТИЖНЯ (Вогник активності): GET /api/journal/activity/weekly?date=2026-04-23
    [HttpGet("activity/weekly")]
    public async Task<IActionResult> GetWeeklyActivity([FromQuery] DateTime date)
    {
        var query = new GetWeeklyActivityQuery(GetUserId(), date);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    public record UpdateFoodEntryRequest(decimal Grams);
}