namespace ReferralProgram.Servercore.Models;

public class Referral
{
    public int Id { get; set; }
    public string ReferrerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    public bool IsRedeemed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RedeemedAt { get; set; }
}
