using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ReferralProgram.Servercore.Data;
using ReferralProgram.Servercore.DTOs;

namespace ReferralProgram.Tests;

[TestFixture]
public class ReferralControllerIntegrationTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task CreateReferral_WithValidData_ReturnsSuccessAndSavesToDatabase()
    {
        // Arrange
        var request = new CreateReferralRequest
        {
            Name = "Martin",
            PhoneNumber = "0412345678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/referral", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<CreateReferralResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.True);
        Assert.That(result.ReferralCode, Is.Not.Null.And.Not.Empty);
        Assert.That(result.ReferralCode, Does.StartWith("MARTIN-"));

        // Verify entity was saved to database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
        var savedReferral = await dbContext.Referrals.FindAsync(1);

        Assert.That(savedReferral, Is.Not.Null);
        Assert.That(savedReferral!.ReferrerName, Is.EqualTo("Martin"));
        Assert.That(savedReferral.PhoneNumber, Is.EqualTo("0412345678"));
        Assert.That(savedReferral.IsRedeemed, Is.False);
    }

    [Test]
    public async Task CreateReferral_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateReferralRequest
        {
            Name = "",
            PhoneNumber = "0412345678"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/referral", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var result = await response.Content.ReadFromJsonAsync<CreateReferralResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Name and phone number are required."));
    }

    [Test]
    public async Task CreateReferral_WithEmptyPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateReferralRequest
        {
            Name = "Martin",
            PhoneNumber = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/referral", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var result = await response.Content.ReadFromJsonAsync<CreateReferralResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
    }

    [Test]
    public async Task CreateReferral_WithDuplicatePhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateReferralRequest
        {
            Name = "Martin",
            PhoneNumber = "0412345678"
        };

        // Create first referral
        await _client.PostAsJsonAsync("/api/referral", request);

        // Act - try to create second referral with same phone number
        var request2 = new CreateReferralRequest
        {
            Name = "John",
            PhoneNumber = "0412345678"
        };
        var response = await _client.PostAsJsonAsync("/api/referral", request2);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var result = await response.Content.ReadFromJsonAsync<CreateReferralResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(result.Message, Does.Contain("phone number"));
    }

    [Test]
    public async Task GetReferralByCode_WithValidCode_ReturnsReferralDetails()
    {
        // Arrange - create a referral first
        var createRequest = new CreateReferralRequest
        {
            Name = "Sarah",
            PhoneNumber = "0498765432"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/referral", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateReferralResponse>();
        var referralCode = createResult!.ReferralCode;

        // Act
        var response = await _client.GetAsync($"/api/referral/{referralCode}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<ReferralDetailsResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ReferrerName, Is.EqualTo("Sarah"));
        Assert.That(result.PhoneNumber, Is.EqualTo("0498765432"));
        Assert.That(result.ReferralCode, Is.EqualTo(referralCode));
        Assert.That(result.IsRedeemed, Is.False);
    }

    [Test]
    public async Task GetReferralByCode_WithInvalidCode_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/referral/INVALID-CODE");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
