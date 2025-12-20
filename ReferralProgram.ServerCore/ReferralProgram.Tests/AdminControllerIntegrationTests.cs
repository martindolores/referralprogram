using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ReferralProgram.Servercore.Data;
using ReferralProgram.Servercore.DTOs;

namespace ReferralProgram.Tests;

[TestFixture]
public class AdminControllerIntegrationTests
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
    public async Task MarkAsRedeemed_WithValidCode_ReturnsSuccessAndUpdatesDatabase()
    {
        // Arrange - create a referral first
        var createRequest = new CreateReferralRequest
        {
            Name = "Martin",
            PhoneNumber = "0412345678"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/referral", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateReferralResponse>();
        var referralCode = createResult!.ReferralCode;

        var redeemRequest = new MarkRedeemedRequest
        {
            ReferralCode = referralCode!
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/redeem", redeemRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var result = await response.Content.ReadFromJsonAsync<MarkRedeemedResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.True);
        Assert.That(result.Message, Does.Contain("successfully"));

        // Verify entity was updated in database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
        var updatedReferral = await dbContext.Referrals.FindAsync(1);

        Assert.That(updatedReferral, Is.Not.Null);
        Assert.That(updatedReferral!.IsRedeemed, Is.True);
        Assert.That(updatedReferral.RedeemedAt, Is.Not.Null);
    }

    [Test]
    public async Task MarkAsRedeemed_WithInvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var request = new MarkRedeemedRequest
        {
            ReferralCode = "INVALID-CODE"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/redeem", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var result = await response.Content.ReadFromJsonAsync<MarkRedeemedResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(result.Message, Does.Contain("not found"));
    }

    [Test]
    public async Task MarkAsRedeemed_WithEmptyCode_ReturnsBadRequest()
    {
        // Arrange
        var request = new MarkRedeemedRequest
        {
            ReferralCode = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/redeem", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var result = await response.Content.ReadFromJsonAsync<MarkRedeemedResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Referral code is required."));
    }

    [Test]
    public async Task MarkAsRedeemed_WhenAlreadyRedeemed_ReturnsBadRequest()
    {
        // Arrange - create and redeem a referral
        var createRequest = new CreateReferralRequest
        {
            Name = "Sarah",
            PhoneNumber = "0498765432"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/referral", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateReferralResponse>();
        var referralCode = createResult!.ReferralCode;

        var redeemRequest = new MarkRedeemedRequest
        {
            ReferralCode = referralCode!
        };

        // First redemption
        await _client.PostAsJsonAsync("/api/admin/redeem", redeemRequest);

        // Act - try to redeem again
        var response = await _client.PostAsJsonAsync("/api/admin/redeem", redeemRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var result = await response.Content.ReadFromJsonAsync<MarkRedeemedResponse>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already redeemed"));
    }

    [Test]
    public async Task FullWorkflow_CreateReferralThenRedeemThenVerify()
    {
        // Step 1: Create referral
        var createRequest = new CreateReferralRequest
        {
            Name = "Lorna",
            PhoneNumber = "0411111111"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/referral", createRequest);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateReferralResponse>();
        var referralCode = createResult!.ReferralCode;

        // Step 2: Verify referral is not redeemed
        var getResponse1 = await _client.GetAsync($"/api/referral/{referralCode}");
        var getResult1 = await getResponse1.Content.ReadFromJsonAsync<ReferralDetailsResponse>();
        Assert.That(getResult1!.IsRedeemed, Is.False);

        // Step 3: Mark as redeemed
        var redeemRequest = new MarkRedeemedRequest { ReferralCode = referralCode! };
        var redeemResponse = await _client.PostAsJsonAsync("/api/admin/redeem", redeemRequest);
        Assert.That(redeemResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // Step 4: Verify referral is now redeemed
        var getResponse2 = await _client.GetAsync($"/api/referral/{referralCode}");
        var getResult2 = await getResponse2.Content.ReadFromJsonAsync<ReferralDetailsResponse>();
        Assert.That(getResult2!.IsRedeemed, Is.True);
        Assert.That(getResult2.RedeemedAt, Is.Not.Null);

        // Step 5: Verify in database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
        var dbReferral = dbContext.Referrals.First(r => r.ReferralCode == referralCode);

        Assert.That(dbReferral.ReferrerName, Is.EqualTo("Lorna"));
        Assert.That(dbReferral.IsRedeemed, Is.True);
        Assert.That(dbReferral.RedeemedAt, Is.Not.Null);
    }
}
