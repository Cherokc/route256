using System.Threading.Tasks;
using AutoBogus;
using FluentAssertions;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.ServiceTests;

[Collection(nameof(TestFixture))]
public class RateLimiterServiceTests
{
    private readonly IRateLimiterService _rateLimiterService;

    public RateLimiterServiceTests(TestFixture testFixture)
    {
        _rateLimiterService = testFixture.RateLimiterService;
    }

    [Fact]
    public async Task Allow_SingleRequest_ReturnTrue()
    {
        // Arrange
        var clientIp = AutoFaker.Generate<string>();
        
        // Act
        var actual = await _rateLimiterService.Allow(clientIp);

        // Assert
        actual.Should().BeTrue();
    }
    
    [Fact]
    public async Task Allow_ManyRequests_ReturnFalse()
    {
        // Arrange
        const int attempts = 100;
        var clientIp = AutoFaker.Generate<string>();
        
        for (int i = 0; i < attempts; i++)
        {
            await _rateLimiterService.Allow(clientIp);
        }
        
        // Act
        var actual = await _rateLimiterService.Allow(clientIp);

        // Assert
        actual.Should().BeFalse();
    }
}