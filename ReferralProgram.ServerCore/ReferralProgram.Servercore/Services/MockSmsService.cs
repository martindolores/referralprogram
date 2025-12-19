namespace ReferralProgram.Servercore.Services;

public class MockSmsService : ISmsService
{
    private readonly ILogger<MockSmsService> _logger;

    public MockSmsService(ILogger<MockSmsService> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendReferralCodeAsync(string phoneNumber, string name, string referralCode)
    {
        _logger.LogInformation(
            "Mock SMS sent to {PhoneNumber}: Your referral code is {ReferralCode}! Tell your friend to mention it when they DM us. You'll both get 10% off!",
            phoneNumber,
            referralCode
        );

        return Task.FromResult(true);
    }
}
