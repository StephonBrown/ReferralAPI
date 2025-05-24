using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Repositories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Livefront.Referrals.UnitTests.BusinessLogic.Services;

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
    protected void GivenUserRepositoryGetByUserIdBySpecificUserIdReturnsUser(Guid userId, User user)
    {
        mockedUserRepository
            .GetById(Arg.Is<Guid>(id => id == userId), 
                Arg.Any<CancellationToken>())
            .Returns(user);
    }
    
    protected void GivenUserRepositoryGetByIdsReturnsUsers(IList<User> users)
    {
        var userIds = users.Select(user => user.Id).ToHashSet();
        mockedUserRepository
            .GetByIds(Arg.Is<IEnumerable<Guid>>(ids => ids.ToHashSet().SetEquals(userIds)), 
                Arg.Any<CancellationToken>())
            .Returns(users);
    }
    
    protected void GivenUserRepositoryGetByIdsReturnsEmpty()
    {
        mockedUserRepository
            .GetByIds(Arg.Any<IEnumerable<Guid>>(),
                Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<User>());
    }
    
    protected async Task ThenUserRepositoryGetByIdsShouldBeCalled(IEnumerable<Guid> userIds, int numberOfCalls)
    {
        await mockedUserRepository
            .Received(numberOfCalls)
            .GetByIds(Arg.Is<IEnumerable<Guid>>(ids => ids.ToHashSet().SetEquals(userIds.ToHashSet())), 
                Arg.Is<CancellationToken>(ct => ct == cancellationToken));
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