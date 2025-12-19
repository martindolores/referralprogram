using Microsoft.AspNetCore.Mvc;
using ReferralProgram.Servercore.DTOs;
using ReferralProgram.Servercore.Services;

namespace ReferralProgram.Servercore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IReferralService _referralService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IReferralService referralService, ILogger<AdminController> logger)
    {
        _referralService = referralService;
        _logger = logger;
    }

    [HttpPost("redeem")]
    public async Task<ActionResult<MarkRedeemedResponse>> MarkAsRedeemed([FromBody] MarkRedeemedRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReferralCode))
        {
            return BadRequest(new MarkRedeemedResponse
            {
                Success = false,
                Message = "Referral code is required."
            });
        }

        try
        {
            var success = await _referralService.MarkAsRedeemedAsync(request.ReferralCode.Trim());

            if (!success)
            {
                return BadRequest(new MarkRedeemedResponse
                {
                    Success = false,
                    Message = "Referral code not found or already redeemed."
                });
            }

            return Ok(new MarkRedeemedResponse
            {
                Success = true,
                Message = "Referral code marked as redeemed successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking referral as redeemed for {ReferralCode}", request.ReferralCode);
            return StatusCode(500, new MarkRedeemedResponse
            {
                Success = false,
                Message = "An error occurred while marking the referral as redeemed."
            });
        }
    }
}
