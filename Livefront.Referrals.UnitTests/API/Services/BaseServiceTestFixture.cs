using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Livefront.Referrals.UnitTests.API.Services;

public class BaseServiceTestFixture
{
    protected IUserRepository mockedUserRepository = Substitute.For<IUserRepository>();
    protected CancellationToken cancellationToken = CancellationToken.None;

    protected void GivenUserRepositoryGetByIdReturnsNull()
    {
        mockedUserRepository
            .GetById(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .ReturnsNull();
    }
    
    protected async Task ThenUserRepositoryGetByIdShouldBeCalled(Guid userId,  int numberOfCalls)
    {
        await mockedUserRepository
            .Received(numberOfCalls)
            .GetById(Arg.Is<Guid>( id => id == userId ), 
                Arg.Is<CancellationToken>(ct => ct == cancellationToken ));
    }
    
    protected void GivenUserRepositoryGetByUserIdReturnsUser(User user)
    {
        mockedUserRepository
            .GetById(Arg.Any<Guid>(), 
                Arg.Any<CancellationToken>())
            .Returns(user);
    }
    protected async Task ThenUserRespositoryGetByReferralCodeShouldBeCalled(string referralCode, int numberOfCalls)
    {
        await mockedUserRepository
            .Received(numberOfCalls)
            .GetUserByReferralCode(Arg.Is<string>(code => code == referralCode),
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
    }
    protected void GivenUserRepositoryGetByReferralCodeReturnsUser(User referrerUser)
    {
        mockedUserRepository.GetUserByReferralCode(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(referrerUser);
    }
    
    protected void GivenUserRepositoryGetByReferralCodeReturnsNull()
    {
        mockedUserRepository
            .GetUserByReferralCode(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ReturnsNull();
    }
}