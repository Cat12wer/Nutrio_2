using FluentValidation;
using Nutrio.Domain.Exceptions;
using System.Text.Json;

namespace Nutrio.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Пропускаємо запит далі
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Якщо десь вилетіла помилка - ловимо її
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // За замовчуванням ставимо 500 (внутрішня помилка)
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var response = new { error = "Внутрішня помилка сервера." };

        // Якщо це НАША доменна помилка (наприклад, InvalidEmail)
        if (exception is DomainException domainEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = new { error = domainEx.Message };
        }
        // Якщо це помилка валідації вхідних даних (FluentValidation)
        else if (exception is ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errors = string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage));
            response = new { error = errors };
        }
        // Якщо хтось лізе туди, куди не можна
        else if (exception is UnauthorizedAccessException unauthEx)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            response = new { error = unauthEx.Message };
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}