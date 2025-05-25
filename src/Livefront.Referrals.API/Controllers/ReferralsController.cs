using Livefront.Referrals.API.Models;
using Livefront.Referrals.API.Services;
using Livefront.Referrals.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralsController : ControllerBase
    {
        private readonly ILogger<ReferralsController> logger;
        private readonly IReferralService referralService;

        public ReferralsController(ILogger<ReferralsController> logger, IReferralService referralService)
        {
            this.logger = logger;
            this.referralService = referralService;
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult GetReferrals()
        {
            logger.LogInformation("GetReferrals called");
            return Ok();
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteReferral([FromBody]CreateReferralRequest? createReferralRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("CompleteReferral called");
            ValidateCreateReferralRequest(createReferralRequest);
            var referralDto = await referralService.CreateReferral(createReferralRequest!.RefereeId, createReferralRequest.ReferralCode, cancellationToken);
            return Ok(referralDto);
        }

        private void ValidateCreateReferralRequest(CreateReferralRequest? createReferralRequest)
        {
            if (createReferralRequest == null)
            {
                logger.LogWarning("CreateReferralRequest is null");
                throw new ArgumentNullException(nameof(createReferralRequest), "CreateReferralRequest cannot be null");
            }

            if (string.IsNullOrWhiteSpace(createReferralRequest.ReferralCode))
            {
                logger.LogWarning("Referral code is null or empty");
                throw new ArgumentException("Referral code cannot be null or empty",
                    nameof(createReferralRequest.ReferralCode));
            }

            if (createReferralRequest.RefereeId == Guid.Empty)
            {
                logger.LogWarning("Referee user ID is empty");
                throw new ArgumentException("Referee user ID cannot be empty", nameof(createReferralRequest.RefereeId));
            }
        }
    }
}
