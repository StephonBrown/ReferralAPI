using Livefront.Referrals.DataAccess.Models;
using Livefront.Referrals.DataAccess.Services;
using Microsoft.EntityFrameworkCore;

namespace Livefront.Referrals.DataAccess.Repositories;

public class LinkRepository : ILinkRepository
{
    private readonly DbContext referralsContext;
    public LinkRepository(DbContext referralsContext)
    {
        this.referralsContext = referralsContext;
    }

    public async Task<ReferralLink> GetLinkByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<ReferralLink> Create(Guid userId, Uri linkUrl)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException($"{nameof(userId)} must not be empty", nameof(userId));
        }
        
        if (linkUrl == null)
        {
            throw new ArgumentException($"{nameof(linkUrl)} must not me null or empty", nameof(linkUrl));
        }

        try
        {
            //save to data source
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //Log Exception
            throw;
        }    
    }

    public async Task<ReferralLink> Update(Guid userId)
    {
        throw new NotImplementedException();
    }
    
}