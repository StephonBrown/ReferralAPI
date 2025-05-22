namespace Livefront.Referrals.API.Models;

public record Referral(Guid Id, Guid ReferreeId, Guid ReferrerId, ReferralStatus Status, DateTime DateCreated);