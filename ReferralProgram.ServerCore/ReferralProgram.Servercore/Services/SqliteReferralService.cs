using Microsoft.EntityFrameworkCore;
using ReferralProgram.Servercore.Data;
using ReferralProgram.Servercore.Models;

namespace ReferralProgram.Servercore.Services;

public class SqliteReferralService : IReferralService
{
    private readonly ReferralDbContext _context;
    private readonly ILogger<SqliteReferralService> _logger;
    private static readonly Random _random = new();

    public SqliteReferralService(ReferralDbContext context, ILogger<SqliteReferralService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string ReferralCode, string Message)> CreateReferralAsync(string name, string phoneNumber)
    {
        var normalizedPhone = NormalizePhoneNumber(phoneNumber);

        if (await PhoneNumberExistsAsync(normalizedPhone))
        {
            return (false, string.Empty, "This phone number has already been used for a referral.");
        }

        var referralCode = GenerateReferralCode(name);

        var referral = new Referral
        {
            ReferrerName = name.Trim(),
            PhoneNumber = normalizedPhone,
            ReferralCode = referralCode,
            IsRedeemed = false,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created referral {ReferralCode} for {Name}", referralCode, name);
            return (true, referralCode, "Referral created successfully.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to create referral for {Name}", name);
            return (false, string.Empty, "Failed to create referral. Please try again.");
        }
    }

    public async Task<Referral?> GetReferralByCodeAsync(string referralCode)
    {
        return await _context.Referrals
            .FirstOrDefaultAsync(r => r.ReferralCode == referralCode.Trim().ToUpperInvariant());
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

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Marked referral {ReferralCode} as redeemed", referralCode);
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to mark referral {ReferralCode} as redeemed", referralCode);
            return false;
        }
    }

    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
    {
        var normalizedPhone = NormalizePhoneNumber(phoneNumber);
        return await _context.Referrals.AnyAsync(r => r.PhoneNumber == normalizedPhone);
    }

    private static string GenerateReferralCode(string name)
    {
        var namePart = name.Trim().Split(' ')[0].ToUpperInvariant();
        if (namePart.Length > 8)
        {
            namePart = namePart[..8];
        }

        var randomPart = _random.Next(1000, 9999);
        return $"{namePart}-{randomPart}";
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }
}
