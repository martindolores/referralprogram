using Microsoft.AspNetCore.Mvc;
using ReferralProgram.Servercore.DTOs;
using ReferralProgram.Servercore.Services;

namespace ReferralProgram.Servercore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _referralService;
    private readonly ILogger<ReferralController> _logger;

    public ReferralController(IReferralService referralService, ILogger<ReferralController> logger)
    {
        _referralService = referralService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<CreateReferralResponse>> CreateReferral([FromBody] CreateReferralRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return BadRequest(new CreateReferralResponse
            {
                Success = false,
                Message = "Name and phone number are required."
            });
        }

        try
        {
            var (success, referralCode, message) = await _referralService.CreateReferralAsync(
                request.Name.Trim(),
                request.PhoneNumber.Trim()
            );

            if (!success)
            {
                return BadRequest(new CreateReferralResponse
                {
                    Success = false,
                    Message = message
                });
            }

            return Ok(new CreateReferralResponse
            {
                Success = true,
                Message = message,
                ReferralCode = referralCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating referral for {Name}", request.Name);
            return StatusCode(500, new CreateReferralResponse
            {
                Success = false,
                Message = "An error occurred while creating the referral."
            });
        }
    }

    [HttpGet("{referralCode}")]
    public async Task<ActionResult<ReferralDetailsResponse>> GetReferralByCode(string referralCode)
    {
        try
        {
            var referral = await _referralService.GetReferralByCodeAsync(referralCode);

            if (referral == null)
            {
                return NotFound(new { Message = "Referral code not found." });
            }

            return Ok(new ReferralDetailsResponse
            {
                ReferrerName = referral.ReferrerName,
                PhoneNumber = referral.PhoneNumber,
                ReferralCode = referral.ReferralCode,
                IsRedeemed = referral.IsRedeemed,
                CreatedAt = referral.CreatedAt,
                RedeemedAt = referral.RedeemedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching referral code {ReferralCode}", referralCode);
            return StatusCode(500, new { Message = "An error occurred while fetching the referral." });
        }
    }
}
