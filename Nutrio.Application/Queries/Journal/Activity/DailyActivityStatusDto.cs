// Nutrio.Application/Queries/Journal/Activity/WeeklyActivityDto.cs
namespace Nutrio.Application.Queries.Journal.Activity;

public record DailyActivityStatusDto(
    DateTime Date,
    bool HasEntries 
);

public record WeeklyActivityDto(
    int CurrentStreak, 
    List<DailyActivityStatusDto> WeekDays
);