using Livefront.Referrals.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralsController : ControllerBase
    {
        private readonly ILogger<ReferralsController> logger;

        public ReferralsController(ILogger<ReferralsController> logger)
        {
            this.logger = logger;
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
        public IActionResult CompleteReferral([FromBody]CreateReferralRequest createReferralRequest)
        {
            logger.LogInformation("CreateReferral called");
            return Ok(createReferralRequest);
        }
        
    }
}
