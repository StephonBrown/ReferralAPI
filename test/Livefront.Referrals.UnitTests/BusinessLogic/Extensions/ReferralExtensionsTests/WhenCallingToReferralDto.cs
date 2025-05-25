using Livefront.BusinessLogic.Extensions;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Extensions.ReferralExtensionsTests;

[TestFixture]
public class WhenCallingToReferralDto
{
    [Test]
    public void GoldenPath()
    {
        // Arrange
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = Guid.NewGuid(),
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        var referee = new User
        {
            Id = referral.RefereeId,
            FirstName = "John",
            LastName = "Doe",
            Email = "John@email.com",
            ReferralCode = "JOHNCODE"
        };

        // Act
        var result = referral.ToReferralDto(referee);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.That(referral.Id, Is.EqualTo(result.ReferralId));
        Assert.That(referee.Id, Is.EqualTo(result.UserId));
        Assert.That(referee.FirstName, Is.EqualTo(result.FirstName));
        Assert.That(referee.LastName, Is.EqualTo(result.LastName));
    }
    
    [Test]
    public void GivenReferralIsNull_ThenThrowArgumentNullException()
    {
        // Arrange
        Referral? referral = null;
        var referee = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "John@email.com",
            ReferralCode = "JOHNCODE"
        };
        // Act/Assert
        var exception = Assert.Throws<ArgumentNullException>(() => referral!.ToReferralDto(referee));
        Assert.That(exception!.ParamName, Is.EqualTo("referral"));
    }

    [Test]
    public void GivenReferralIdIsEmpty_ThrowArgumentException()
    {
        // Arrange
        var referral = new Referral
        {
            Id = Guid.Empty,
        };
        var referee = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "John@email.com",
            ReferralCode = "JOHNCODE"
        };
        // Act/Assert
        var exception = Assert.Throws<ArgumentException>(() => referral.ToReferralDto(referee!));
        Assert.That(exception!.ParamName, Is.EqualTo("referral"));
    }
    
    [Test]
    public void GivenRefereeIsNull_ThenThrowArgumentNullException()
    {
        // Arrange
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = Guid.NewGuid(),
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        User? referee = null;
        
        // Act/Assert
        var exception = Assert.Throws<ArgumentNullException>(() => referral.ToReferralDto(referee!));
        Assert.That(exception!.ParamName, Is.EqualTo("referee"));
    }

    [Test]
    public void GivenRefereeIdIsEmpty_ThenThrowArgumentException()
    {
        // Arrange
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = Guid.NewGuid(),
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        var referee = new User
        {
            Id = Guid.Empty,
            FirstName = "John",
            LastName = "Doe",
            Email = "John@email.com",
            ReferralCode = "JOHNCODE"
        };
        // Act/Assert
        var exception = Assert.Throws<ArgumentException>(() => referral.ToReferralDto(referee!));
        Assert.That(exception!.ParamName, Is.EqualTo("referee"));
    }

    [Test]
    public void GivenRefereeFirstNameIsInvalid_ThenThrowArgumentException()
    {
        // Arrange
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = Guid.NewGuid(),
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        var referee = new User
        {
            Id = referral.RefereeId,
            FirstName = "",
            LastName = "Doe",
            Email = "John@email.com",
            ReferralCode = "JOHNCODE"
        };
        // Act/Assert
        var exception = Assert.Throws<ArgumentException>(() => referral.ToReferralDto(referee!));
        Assert.That(exception!.ParamName, Is.EqualTo("referee"));
    }
    
    [Test]
    public void GivenRefereeLastNameIsInvalid_ThenThrowArgumentException()
    {
        // Arrange
        var referral = new Referral
        {
            Id = Guid.NewGuid(),
            ReferrerId = Guid.NewGuid(),
            RefereeId = Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            Status = ReferralStatus.Complete
        };
        var referee = new User
        {
            Id = referral.RefereeId,
            FirstName = "John",
            LastName = "",
            Email = "John@email.com",
            ReferralCode = "JOHNCODE"
        };
        // Act/Assert
        var exception = Assert.Throws<ArgumentException>(() => referral.ToReferralDto(referee!));
        Assert.That(exception!.ParamName, Is.EqualTo("referee"));
    }
    

}