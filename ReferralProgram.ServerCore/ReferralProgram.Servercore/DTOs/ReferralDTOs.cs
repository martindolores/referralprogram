namespace ReferralProgram.Servercore.DTOs;

public class CreateReferralRequest
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class CreateReferralResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ReferralCode { get; set; }
}

public class ReferralDetailsResponse
{
    public string ReferrerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    public bool IsRedeemed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RedeemedAt { get; set; }
}

public class MarkRedeemedRequest
{
    public string ReferralCode { get; set; } = string.Empty;
}

public class MarkRedeemedResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
