using Livefront.BusinessLogic.Models;
using Livefront.Referrals.DataAccess.Models;

namespace Livefront.BusinessLogic.Extensions;

public static class UserExtensions
{
    public static UserDTO ToUserDTO(this User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        return new UserDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ReferralCode = user.ReferralCode
        };
    }
}