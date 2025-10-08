using FluentAssertions;
using MentalHealthCheckinApi.Services;
using Xunit;

namespace MentalHealthCheckinApi.Tests.Services;

public class CheckInServiceTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void IsValidMood_ReturnsTrue_ForValuesWithinRange(int mood)
    {
        var svc = new CheckInService();
        svc.IsValidMood(mood).Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void IsValidMood_ReturnsFalse_ForValuesOutsideRange(int mood)
    {
        var svc = new CheckInService();
        svc.IsValidMood(mood).Should().BeFalse();
    }
}
