using Livefront.Referrals.API.Extensions;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.UnitTests.API.Extensions.ReferralLinkExtensionsTests;

[TestFixture]
public class WhenCallingToReferralLinkDto
{
    [Test]
    public void GoldenPath()
    {
        // Arrange
        var referralLink = new ReferralLink
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BaseDeepLink = "https://example.com",
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            DateCreated = DateTime.UtcNow,
            ThirdPartyId = 43
        };
        // Act
        var result = referralLink.ToReferralLinkDto();
        // Assert
        Assert.IsNotNull(result);
        Assert.That(referralLink.BaseDeepLink, Is.EqualTo(result!.ReferralLink));
        Assert.That(referralLink.ExpirationDate, Is.EqualTo(result.ExpirationDate));
    }
    
    [Test]
    public void GivenReferralLinkIsNull_ThenReturnNull()
    {
        // Arrange
        ReferralLink? referralLink = null;

        // Act
        var result = referralLink.ToReferralLinkDto();

        // Assert
        Assert.IsNull(result);
    }
    
    
    
}