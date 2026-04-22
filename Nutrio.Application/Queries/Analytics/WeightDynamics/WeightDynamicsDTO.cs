namespace Nutrio.Application.Queries.Analytics.WeightDynamics;

// DTO для окремої точки на графіку (один замір)
public record WeightPointDTO(
    DateTime Date,
    decimal Weight
);

// Головний DTO для віджета динаміки ваги
public record WeightDynamicsDTO(
    decimal TargetWeight, // Цільова вага користувача (для малювання лінії цілі)
    List<WeightPointDTO> History // Масив точок (дата + вага) для побудови графіка
);