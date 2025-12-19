using ReferralProgram.Servercore.Models;

namespace ReferralProgram.Servercore.Services;

public class MockReferralService : IReferralService
{
    private readonly ISmsService _smsService;
    private readonly List<Referral> _mockDatabase = new();
    private int _nextId = 1;

    public MockReferralService(ISmsService smsService)
    {
        _smsService = smsService;
    }

    public async Task<(bool Success, string ReferralCode, string Message)> CreateReferralAsync(string name, string phoneNumber)
    {
        if (await PhoneNumberExistsAsync(phoneNumber))
        {
            return (false, string.Empty, "This phone number has already been used for a referral.");
        }

        var referralCode = GenerateReferralCode(name);
        
        var smsSent = await _smsService.SendReferralCodeAsync(phoneNumber, name, referralCode);
        
        if (!smsSent)
        {
            return (false, string.Empty, "Failed to send SMS. Please check the phone number and try again.");
        }

        var referral = new Referral
        {
            Id = _nextId++,
            ReferrerName = name,
            PhoneNumber = phoneNumber,
            ReferralCode = referralCode,
            IsRedeemed = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockDatabase.Add(referral);

        return (true, referralCode, "Referral code sent successfully!");
    }

    public Task<Referral?> GetReferralByCodeAsync(string referralCode)
    {
        var referral = _mockDatabase.FirstOrDefault(r => 
            r.ReferralCode.Equals(referralCode, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(referral);
    }

    public async Task<bool> MarkAsRedeemedAsync(string referralCode)
    {
        var referral = await GetReferralByCodeAsync(referralCode);
        
        if (referral == null || referral.IsRedeemed)
        {
            return false;
        }

        referral.IsRedeemed = true;
        referral.RedeemedAt = DateTime.UtcNow;
        return true;
    }

    public Task<bool> PhoneNumberExistsAsync(string phoneNumber)
    {
        var exists = _mockDatabase.Any(r => r.PhoneNumber == phoneNumber);
        return Task.FromResult(exists);
    }

    private string GenerateReferralCode(string name)
    {
        var namePart = new string(name.Where(char.IsLetter).Take(6).ToArray()).ToUpper();
        if (string.IsNullOrEmpty(namePart))
        {
            namePart = "USER";
        }
        
        var random = new Random();
        var numberPart = random.Next(1000, 9999);
        
        return $"{namePart}-{numberPart}";
    }
}
