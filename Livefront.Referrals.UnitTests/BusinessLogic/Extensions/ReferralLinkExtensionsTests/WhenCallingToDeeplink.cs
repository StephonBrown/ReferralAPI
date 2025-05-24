using Livefront.BusinessLogic.Extensions;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Extensions.ReferralLinkExtensionsTests;

[TestFixture]
public class WhenCallingToDeeplink
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
            ThirdPartyId = 33
        };
        
        // Act
        var result = referralLink.ToDeepLink();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.That(referralLink.BaseDeepLink, Is.EqualTo(result!.Link));
        Assert.That(referralLink.ExpirationDate, Is.EqualTo(result.ExpirationDate));
        Assert.That(referralLink.DateCreated, Is.EqualTo(result.DateCreated));
        Assert.That(referralLink.ThirdPartyId, Is.EqualTo(result.Id));
    }
    
    [Test]
    public void GivenReferralLinkIsNull_ThenReturnNull()
    {
        // Arrange
        ReferralLink? referralLink = null;

        // Act
        var result = referralLink.ToDeepLink();

        // Assert
        Assert.IsNull(result);
    }
}