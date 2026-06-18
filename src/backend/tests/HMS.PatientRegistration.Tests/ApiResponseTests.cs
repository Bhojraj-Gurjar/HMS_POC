using HMS.PatientRegistration.Application.Common.Models;
using HMS.PatientRegistration.Application.Common.Settings;

namespace HMS.PatientRegistration.Tests;

public class ApiResponseTests
{
    [Fact]
    public void Ok_CreatesSuccessfulEnvelope()
    {
        var response = ApiResponse<string>.Ok("data", "ok", "corr-1");

        Assert.True(response.Success);
        Assert.Equal("data", response.Data);
        Assert.Equal("ok", response.Message);
        Assert.Equal("corr-1", response.CorrelationId);
    }
}

public class DataModeSettingsTests
{
    [Theory]
    [InlineData("Mock", true, false)]
    [InlineData("SqlServer", false, true)]
    [InlineData("Database", false, true)]
    public void Mode_ParsesCorrectly(string mode, bool isMock, bool isDatabase)
    {
        var settings = new DataModeSettings { Mode = mode };

        Assert.Equal(isMock, settings.IsMock);
        Assert.Equal(isDatabase, settings.IsDatabase);
    }
}
