namespace MentalHealthCheckinApi.Dtos;

public record CheckInDto(
    Guid Id,
    Guid UserId,
    string Username,
    int Mood,
    string? Notes,
    DateTimeOffset CreatedAt
);
