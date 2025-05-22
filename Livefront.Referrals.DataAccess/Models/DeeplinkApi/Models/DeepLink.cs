namespace Livefront.Referrals.DataAccess.Models.DeeplinkApi.Models;

public class DeepLink
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public string Link { get; set; }
    public DateTime ExpirationDate { get; set; }

    public DeepLink(int id, DateTime dateCreated, DateTime expirationDate, string link)
    {
        Id = id;
        DateCreated = dateCreated;
        ExpirationDate = expirationDate;
        Link = link;
    }
    public DeepLink(){}
}