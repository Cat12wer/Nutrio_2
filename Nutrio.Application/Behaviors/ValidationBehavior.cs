using FluentValidation;
using MediatR;

namespace Nutrio.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> // Обмежуємо, щоб це працювало тільки для запитів MediatR
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // Впроваджуємо всі валідатори, які ми створимо для конкретної команди
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Запускаємо всі валідатори асинхронно
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Збираємо всі помилки докупи
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                // Якщо є хоча б одна помилка, викидаємо виключення FluentValidation
                // Запит зупиняється ТУТ і не йде до CommandHandler
                throw new ValidationException(failures);
            }
        }

        // Якщо помилок немає, передаємо естафету далі (нашому CommandHandler)
        return await next();
    }
}