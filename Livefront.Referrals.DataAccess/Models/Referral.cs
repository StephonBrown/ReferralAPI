using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Livefront.Referrals.DataAccess.Models;

// This adds a unique constraint for both the referrer and referee ids
// both must be unique so no duplicates can arise in the referrals table
[Index(nameof(ReferrerId), nameof(RefereeId), IsUnique = true)]
[Table("Referrals")]
public class Referral
{
    public Guid Id { get; init; }
    [Required]
    public Guid RefereeId { get; init; }
    [Required]
    public Guid ReferrerId { get; init; }
    [Required]
    public ReferralStatus Status { get; init; }
    [Required]
    public DateTime DateCreated { get; init; }


}