namespace MentalHealthCheckinApi.Services;

public interface ICheckInService
{
    bool IsValidMood(int mood); // 1..5
}

public class CheckInService : ICheckInService
{
    public bool IsValidMood(int mood) => mood is >= 1 and <= 5;
}
