using MediatR;
using Nutrio.Domain.Entities;
using Nutrio.Domain.Interfaces;
using Nutrio.Domain.ValueObjects;

namespace Nutrio.Application.Commands.Journal.AddFoodEntry;

public class AddFoodEntryCommandHandler : IRequestHandler<AddFoodEntryCommand, Guid>
{
    private readonly IFoodEntryRepository _foodEntryRepository;
    private readonly IProductRepository _productRepository;

    public AddFoodEntryCommandHandler(
        IFoodEntryRepository foodEntryRepository,
        IProductRepository productRepository)
    {
        _foodEntryRepository = foodEntryRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(AddFoodEntryCommand request, CancellationToken cancellationToken)
    {
        // 1. Перевіряємо, чи існує такий продукт
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
            throw new KeyNotFoundException("Продукт не знайдено в базі даних.");

        // 2. Створюємо Value Object для кількості (наша доменна валідація)
        var quantity = new Quantity(request.Grams);

        // 3. Створюємо нову сутність FoodEntry
        var foodEntry = new FoodEntry(
            request.UserId,
            request.ProductId,
            request.Date.Date, // Зберігаємо тільки дату без часу, щоб легше було шукати
            quantity,
            request.MealType
        );

        // 4. Зберігаємо в базу
        await _foodEntryRepository.AddAsync(foodEntry);

        return foodEntry.Id;
    }
}