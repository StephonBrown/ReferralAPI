using Livefront.Referrals.DataAccess.Exceptions;
using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Livefront.Referrals.DataAccess.Repositories;

public class ReferralRepository : IReferralRepository
{
    private readonly ReferralsContext referralsContext;
    private readonly ILogger<IReferralRepository> logger;
    
    public ReferralRepository(ReferralsContext referralsContext, ILogger<IReferralRepository> logger)
    {
        this.referralsContext = referralsContext;
        this.logger = logger;
    }
    
    /// <inheritdoc />
    public async Task<Referral?> GetById(Guid referralId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Referral>> GetReferralsByReferrerId(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public async Task<Referral> Create(Referral referral, CancellationToken cancellationToken)
    {
        ValidateReferral(referral);
        
        var contextualLogger = Log
            .ForContext<ReferralRepository>()
            .ForContext("Referral", referral);

        try
        {
            await referralsContext.Referrals.AddAsync(referral, cancellationToken);
            await referralsContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created referral {ReferralId}", referral.Id);
            return referral;
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.Message.Contains("duplicate key"))
            {
                contextualLogger
                    .Error(dbUpdateException, 
                        "Referral already exists for referrer {@ReferrerId} and referee {@RefereeId}", referral.ReferrerId, referral.RefereeId);
            }
            throw new ReferralAlreadyExistsException($"Referral already exists for referrer and referee ", dbUpdateException);
        }
        catch (Exception e)
        {
            contextualLogger.Error(e, "Failed to create a referral");
            throw new DataPersistenceException("Failed to create a referral", e);
        }    }
    
    /// <inheritdoc />
    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    private void ValidateReferral(Referral referral)
    {
        if (referral == null)
        {
            logger.LogInformation("Referral is null");
            throw new ArgumentNullException(nameof(referral), "Referral is null");
        }
        
        if (referral.ReferrerId == Guid.Empty)
        {
            logger.LogWarning("Referrer cannot be empty");
            throw new ArgumentException("Referrer must not be empty", nameof(referral));
        }
        
        if (referral.RefereeId == Guid.Empty)
        {
            logger.LogWarning("Referee cannot be empty");
            throw new ArgumentException("Referee must not be empty", nameof(referral));
        }
        
        if (referral.DateCreated == default)
        {
            logger.LogWarning("DateCreated is invalid");
            throw new ArgumentException("DateCreated is invalid", nameof(referral));
        }
        
        if(string.IsNullOrWhiteSpace(referral.ReferralCode))
        {
            logger.LogWarning("ReferralCode is invalid");
            throw new ArgumentException("ReferralCode is invalid", nameof(referral));
        }
        
    }
}