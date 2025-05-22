using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Livefront.Referrals.DataAccess.Repositories;

public class ReferralLinkRepository : IReferralLinkRepository
{
    private readonly ReferralsContext referralsContext;
    private readonly ILogger<IReferralLinkRepository> logger;

    public ReferralLinkRepository(ReferralsContext referralsContext, ILogger<IReferralLinkRepository> logger)
    {
        this.referralsContext = referralsContext;
        this.logger = logger;
    }

    public async Task<ReferralLink?> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning("UserId cannot be empty");
            throw new ArgumentException("UserId must not be empty", nameof(userId));
        }    
        
        try
        {
            return await referralsContext
                .ReferralLinks
                .SingleOrDefaultAsync(link => link.UserId == userId, cancellationToken);
        }
        catch (Exception e)
        {
            var contextualLogger = Log
                .ForContext<ReferralLinkRepository>()
                .ForContext("UserId", userId);
            contextualLogger.Error(e, "Failed to get referral link");
            throw new DataPersistenceException("Failed to create a referral link", e);
        }
    }

    public async Task<ReferralLink> Create(ReferralLink referralLink, CancellationToken cancellationToken)
    {
        
        ValidateReferralLink(referralLink);
        var contextualLogger = Log
            .ForContext<ReferralLinkRepository>()
            .ForContext("ReferralLink", referralLink);

        try
        {
            await referralsContext.ReferralLinks.AddAsync(referralLink, cancellationToken);
            await referralsContext.SaveChangesAsync(cancellationToken);
            return referralLink;
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.Message.Contains("duplicate key"))
            {
                contextualLogger.Error(dbUpdateException, "Referral link already exists for user {@UserId}", referralLink.UserId);
            }
            throw new ReferralLinkAlreadyExistsException("Referral link already exists for user", dbUpdateException);
        }
        catch (Exception e)
        {
            contextualLogger.Error(e, "Failed to create a referral link");
            throw new DataPersistenceException("Failed to create a referral link", e);
        }
    }
    
    public async Task<ReferralLink?> DeleteByUserId(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning("UserId cannot be empty");
            throw new ArgumentException("UserId must not be empty", nameof(userId));
        }
        
        try
        {
            var existingReferralLink = await referralsContext
                .ReferralLinks
                .SingleOrDefaultAsync(link => link.UserId == userId, cancellationToken);
            
            if (existingReferralLink != null)
            {
                referralsContext.ReferralLinks.Remove(existingReferralLink);
                await referralsContext.SaveChangesAsync(cancellationToken);
            }
            
            return existingReferralLink;
        }
        catch (Exception e)
        {
            var contextualLogger = Log
                .ForContext<ReferralLinkRepository>()
                .ForContext("UserId", userId);
            contextualLogger.Error(e, "Failed to update expiration of the referral link");
            throw new DataPersistenceException("Failed to update expiration of the referral link", e);
        }
        
    }

    public async Task<ReferralLink?> UpdateExpirationDate(Guid userId, DateTime newExpirationDate, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            logger.LogWarning("UserId cannot be empty");
            throw new ArgumentException("UserId must not be empty", nameof(userId));
        }
        
        if (newExpirationDate == default)
        {
            logger.LogWarning("ExpirationDate is invalid");
            throw new ArgumentException("ExpirationDate is invalid", nameof(newExpirationDate));
        }
        
        try
        {
            var existingReferralLink = await referralsContext
                .ReferralLinks
                .SingleOrDefaultAsync(link => link.UserId == userId, cancellationToken);
            
            if (existingReferralLink != null && newExpirationDate > existingReferralLink.ExpirationDate)
            {
                existingReferralLink.ExpirationDate = newExpirationDate;
                await referralsContext.SaveChangesAsync(cancellationToken);
            }
            
            return existingReferralLink;
        }
        catch (Exception e)
        {
            var contextualLogger = Log
                .ForContext<ReferralLinkRepository>()
                .ForContext("UserId", userId);
            contextualLogger.Error(e, "Failed to update expiration of the referral link");
            throw new DataPersistenceException("Failed to update expiration of the referral link", e);
        }
    }
    
    private void ValidateReferralLink(ReferralLink referralLink)
    {
        if (referralLink == null)
        {
            logger.LogInformation("Referral link object is null");
            throw new ArgumentNullException(nameof(referralLink), "Referral link object is null");
        }
        
        if (referralLink.UserId == Guid.Empty)
        {
            logger.LogWarning("UserId cannot be empty");
            throw new ArgumentException("UserId must not be empty", nameof(referralLink));
        }
        
        if (string.IsNullOrEmpty(referralLink.BaseDeepLink))
        {
            logger.LogWarning("Link must not be null or empty");
            throw new ArgumentNullException(nameof(referralLink), "Link must not me null or empty");
        }
        
        if (referralLink.DateCreated == default)
        {
            logger.LogWarning("DateCreated is invalid");
            throw new ArgumentException("DateCreated is invalid", nameof(referralLink));
        }

        if (referralLink.ExpirationDate == default)
        {
            logger.LogWarning("ExpirationDate is invalid");
            throw new ArgumentException("ExpirationDate is invalid", nameof(referralLink));
        }

        if (referralLink.DateCreated > referralLink.ExpirationDate)
        {
            logger.LogWarning("DateCreated is greater than ExpirationDate");
            throw new ArgumentException("DateCreated is greater than ExpirationDate", nameof(referralLink));
        }
    }


    
}