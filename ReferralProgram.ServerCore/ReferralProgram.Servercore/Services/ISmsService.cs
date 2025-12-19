namespace ReferralProgram.Servercore.Services;

public interface ISmsService
{
    Task<bool> SendReferralCodeAsync(string phoneNumber, string name, string referralCode);
}
