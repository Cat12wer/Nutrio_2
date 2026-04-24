using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutrio.Application.Queries.Analytics.CaloriesChart;
using Nutrio.Application.Queries.Analytics.MacrosBalance;
using Nutrio.Application.Queries.Analytics.MealDistribution;
using Nutrio.Application.Queries.Analytics.MonthlyActivity;
using Nutrio.Application.Queries.Analytics.NutritionOverview;
using Nutrio.Application.Queries.Analytics.WeeklyGoalsProgress;
using Nutrio.Application.Queries.Analytics.WeightDynamics;

namespace Nutrio.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AnalyticsController : BaseController
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. ВЕРХНІ КАРТКИ (Середні показники): GET /api/analytics/overview?startDate=...&endDate=...
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetNutritionOverviewQuery(GetUserId(), startDate, endDate);
        return Ok(await _mediator.Send(query));
    }

    // 2. ГРАФІК КАЛОРІЙ: GET /api/analytics/calories-chart?startDate=...&endDate=...
    [HttpGet("calories-chart")]
    public async Task<IActionResult> GetCaloriesChart([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetCaloriesChartQuery(GetUserId(), startDate, endDate);
        return Ok(await _mediator.Send(query));
    }

    // 3. БАЛАНС КБЖВ (Кругова діаграма): GET /api/analytics/macros-balance?startDate=...&endDate=...
    [HttpGet("macros-balance")]
    public async Task<IActionResult> GetMacrosBalance([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetMacrosBalanceQuery(GetUserId(), startDate, endDate);
        return Ok(await _mediator.Send(query));
    }

    // 4. РОЗПОДІЛ ПО ПРИЙОМАХ (Сніданок/Обід...): GET /api/analytics/meal-distribution?startDate=...&endDate=...
    [HttpGet("meal-distribution")]
    public async Task<IActionResult> GetMealDistribution([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetMealDistributionQuery(GetUserId(), startDate, endDate);
        return Ok(await _mediator.Send(query));
    }

    // 5. ЦІЛІ ТИЖНЯ: GET /api/analytics/goals?startDate=...&endDate=...
    [HttpGet("goals")]
    public async Task<IActionResult> GetWeeklyGoals([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetWeeklyGoalsProgressQuery(GetUserId(), startDate, endDate);
        return Ok(await _mediator.Send(query));
    }

    // 6. ДИНАМІКА ВАГИ: GET /api/analytics/weight?startDate=...&endDate=...
    [HttpGet("weight")]
    public async Task<IActionResult> GetWeightDynamics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetWeightDynamicsQuery(GetUserId(), startDate, endDate);
        return Ok(await _mediator.Send(query));
    }

    // 7. АКТИВНІСТЬ ЗА МІСЯЦЬ (Календар): GET /api/analytics/monthly-activity?year=2026&month=4
    [HttpGet("monthly-activity")]
    public async Task<IActionResult> GetMonthlyActivity([FromQuery] int year, [FromQuery] int month)
    {
        var query = new GetMonthlyActivityQuery(GetUserId(), year, month);
        return Ok(await _mediator.Send(query));
    }
}