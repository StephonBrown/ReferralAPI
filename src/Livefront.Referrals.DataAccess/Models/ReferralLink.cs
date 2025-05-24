using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Livefront.Referrals.DataAccess.Models;

// UserId is unique so no duplicate links can exist in this table
[Table("ReferralLinks")]
[Index(nameof(UserId), IsUnique = true)]
public class ReferralLink
{
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public int ThirdPartyId { get; set; }
    [Required]
    public DateTime DateCreated { get; set; }
    [Required]
    [Column(TypeName = "varchar(200)")]
    public string BaseDeepLink { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }
}