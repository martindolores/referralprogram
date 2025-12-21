using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ReferralProgram.Servercore.Services;

public class TwilioSmsService : ISmsService
{
    private readonly TwilioSettings _settings;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IOptions<TwilioSettings> settings, ILogger<TwilioSmsService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
    }

    public async Task<bool> SendReferralCodeAsync(string phoneNumber, string name, string referralCode)
    {
        try
        {
            var message = await MessageResource.CreateAsync(
                to: new PhoneNumber(phoneNumber),
                from: new PhoneNumber(_settings.FromNumber),
                body: $"Hi {name}! Your referral code is {referralCode} ??\n\n" +
                      $"Tell your friend to mention it when they DM us.\n" +
                      $"You'll both get 10% off!\n\n" +
                      $"- Lorna's Baked Delights"
            );

            _logger.LogInformation(
                "SMS sent successfully to {PhoneNumber}, SID: {MessageSid}",
                phoneNumber,
                message.Sid);

            return message.Status != MessageResource.StatusEnum.Failed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}

public class TwilioSettings
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;
}
