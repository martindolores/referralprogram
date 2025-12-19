using ReferralProgram.Servercore.Models;

namespace ReferralProgram.Servercore.Services;

public interface IReferralService
{
    Task<(bool Success, string ReferralCode, string Message)> CreateReferralAsync(string name, string phoneNumber);
    Task<Referral?> GetReferralByCodeAsync(string referralCode);
    Task<bool> MarkAsRedeemedAsync(string referralCode);
    Task<bool> PhoneNumberExistsAsync(string phoneNumber);
}
