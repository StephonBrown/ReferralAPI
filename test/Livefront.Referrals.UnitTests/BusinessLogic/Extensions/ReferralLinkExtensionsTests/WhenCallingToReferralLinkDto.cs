using Livefront.BusinessLogic.Extensions;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Extensions.ReferralLinkExtensionsTests;

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
    public void GivenReferralLinkIsNull_ThenThrowArgumentNullException()
    {
        // Arrange
        ReferralLink? referralLink = null;

        // Act/Assert
        // Passing the null-forgiving operator (!) to the method to make sure null is actually passed
        var exception = Assert.Throws<ArgumentNullException>(() => referralLink!.ToReferralLinkDto());
        Assert.That(exception!.ParamName, Is.EqualTo("referralLink"));
    }
    
    [Test]
    public void GivenReferralLinkBaseDeepLinkIsEmpty_ThenThrowArgumentNullException()
    {
        // Arrange
        ReferralLink referralLink = new()
        { 
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BaseDeepLink = string.Empty,
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            DateCreated = DateTime.UtcNow,
            ThirdPartyId = 43
        };

        // Act/Assert
        var exception = Assert.Throws<ArgumentNullException>(() => referralLink.ToReferralLinkDto());
        Assert.That(exception!.ParamName, Is.EqualTo("referralLink"));
    }
}