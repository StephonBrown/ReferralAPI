using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Livefront.Referrals.DataAccess.Models;

[Table("ReferralLinks")]
public class ReferralLink
{
    public Guid Id { get; set; }
    [Required]
    public int ThirdPartyId { get; set; }
    [Required]
    public DateTime DateCreated { get; set; }
    [Required]
    [Column(TypeName = "varchar(200)")]
    public Uri Link { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }

    public ReferralLink(Guid id, int thirdPartyId, DateTime dateCreated,DateTime expirationDate, Uri link)
    {
        Id = id;
        ThirdPartyId = thirdPartyId;
        DateCreated = dateCreated;
        ExpirationDate = expirationDate;
        Link = link;
        
    }
}