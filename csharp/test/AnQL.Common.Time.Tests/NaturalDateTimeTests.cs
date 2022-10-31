using System;
using FluentAssertions;
using Xunit;

namespace AnQL.Common.Time.Tests;

public class NaturalDateTimeTests
{
    [Fact]
    public void TryConvert_AbsoluteMonthRange_ShouldReturnTrue_AndFromTo()
    {
        const string query = "June 2015";

        NaturalDateTime.TryConvert(query, TimeZoneInfo.Utc, out var from, out var to)
            .Should().BeTrue();

        var expectedFrom = new DateTimeOffset(new DateTime(2015, 6, 1)).ToUniversalTime();
        var expectedTo = new DateTimeOffset(new DateTime(2015, 7, 1)).ToUniversalTime();
        
        from.Should().Be(expectedFrom);
        to.Should().Be(expectedTo);
    }
    
    [Fact]
    public void TryConvert_RelativeYearRange_ShouldReturnTrue_AndFromTo()
    {
        const string query = "Last year";

        NaturalDateTime.TryConvert(query, TimeZoneInfo.Utc, out var from, out var to)
            .Should().BeTrue();

        var year = DateTime.Now.Year;

        var expectedFrom = new DateTimeOffset(new DateTime(year - 1, 1, 1)).ToUniversalTime();
        var expectedTo = new DateTimeOffset(new DateTime(year, 1, 1)).ToUniversalTime();
        
        from.Should().Be(expectedFrom);
        to.Should().Be(expectedTo);
    }
    
    [Fact]
    public void TryConvert_FixedTime_ShouldReturnTrue_AndFromOnly()
    {
        const string query = "7AM on 23rd August 2013";

        NaturalDateTime.TryConvert(query, TimeZoneInfo.Utc, out var from, out var to)
            .Should().BeTrue();
        
        var expectedFrom = new DateTimeOffset(new DateTime(2013, 8, 23, 7, 0, 0)).ToUniversalTime();

        to.Should().BeNull();
        from.Should().Be(expectedFrom);
    }

    [Fact]
    public void TryConvert_InvalidTime_ReturnsFalse()
    {
        NaturalDateTime.TryConvert("Santa's Workshop", TimeZoneInfo.Utc, out var from, out var to)
            .Should().BeFalse();

        from.Should().BeNull();
        to.Should().BeNull();
    }
    
    [Fact]
    public void ClampDate_Second_ShouldSetMinMaxToSecond()
    {
        var value = new DateTimeOffset(2023, 6, 11, 5, 32, 22, 103, TimeSpan.Zero);
        var expectedMin = new DateTimeOffset(2023, 6, 11, 5, 32, 22, 0, TimeSpan.Zero);
        var expectedMax = new DateTimeOffset(2023, 6, 11, 5, 32, 23, 0, TimeSpan.Zero);
        
        var (min, max) = NaturalDateTime.ClampDate(value, TimeUnit.Second);

        min.Should().Be(expectedMin);
        max.Should().Be(expectedMax);
    }
    
    [Fact]
    public void ClampDate_Minute_ShouldSetMinMaxToMinute()
    {
        var value = new DateTimeOffset(2023, 6, 11, 5, 32, 22, TimeSpan.Zero);
        var expectedMin = new DateTimeOffset(2023, 6, 11, 5, 32, 0, TimeSpan.Zero);
        var expectedMax = new DateTimeOffset(2023, 6, 11, 5, 33, 0, TimeSpan.Zero);
        
        var (min, max) = NaturalDateTime.ClampDate(value, TimeUnit.Minute);

        min.Should().Be(expectedMin);
        max.Should().Be(expectedMax);
    }
    
    [Fact]
    public void ClampDate_Hour_ShouldSetMinMaxToHour()
    {
        var value = new DateTimeOffset(2023, 6, 11, 5, 32, 0, TimeSpan.Zero);
        var expectedMin = new DateTimeOffset(2023, 6, 11, 5, 0, 0, TimeSpan.Zero);
        var expectedMax = new DateTimeOffset(2023, 6, 11, 6, 0, 0, TimeSpan.Zero);
        
        var (min, max) = NaturalDateTime.ClampDate(value, TimeUnit.Hour);

        min.Should().Be(expectedMin);
        max.Should().Be(expectedMax);
    }
    
    [Fact]
    public void ClampDate_Day_ShouldSetMinMaxToDay()
    {
        var value = new DateTimeOffset(2023, 6, 11, 5, 0, 0, TimeSpan.Zero);
        var expectedMin = new DateTimeOffset(2023, 6, 11, 0, 0, 0, TimeSpan.Zero);
        var expectedMax = new DateTimeOffset(2023, 6, 12, 0, 0, 0, TimeSpan.Zero);
        
        var (min, max) = NaturalDateTime.ClampDate(value, TimeUnit.Day);

        min.Should().Be(expectedMin);
        max.Should().Be(expectedMax);
    }
    
    [Fact]
    public void ClampDate_Month_ShouldSetMinMaxToMonth()
    {
        var value = new DateTimeOffset(2023, 6, 11, 0, 0, 0, TimeSpan.Zero);
        var expectedMin = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var expectedMax = new DateTimeOffset(2023, 7, 1, 0, 0, 0, TimeSpan.Zero);
        
        var (min, max) = NaturalDateTime.ClampDate(value, TimeUnit.Month);

        min.Should().Be(expectedMin);
        max.Should().Be(expectedMax);
    }
    
    [Fact]
    public void ClampDate_Year_ShouldSetMinMaxToYear()
    {
        var value = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var expectedMin = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var expectedMax = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        var (min, max) = NaturalDateTime.ClampDate(value, TimeUnit.Year);

        min.Should().Be(expectedMin);
        max.Should().Be(expectedMax);
    }
}