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
}