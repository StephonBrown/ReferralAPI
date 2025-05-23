using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Livefront.Referrals.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralLinksController : ControllerBase
    {
        private readonly ILogger<ReferralLinksController> logger;

        public ReferralLinksController(ILogger<ReferralLinksController> logger)
        {
            this.logger = logger;
        }
    
        [Authorize]
        [HttpPost]
        public IActionResult CreateReferralLink(Guid id)
        {
            logger.LogInformation("CreateReferral called");
            return Ok();
        }
        
        [Authorize]
        [HttpPut]
        public IActionResult UpdateExpirationDate(Guid id)
        {
            logger.LogInformation("UpdateReferral called");
            return Ok();
        }
    }
}
