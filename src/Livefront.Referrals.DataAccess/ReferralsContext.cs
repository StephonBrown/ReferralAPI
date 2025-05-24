using System.Collections;
using Livefront.Referrals.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Livefront.Referrals.DataAccess;

public class ReferralsContext : DbContext
{
    public string DbPath { get; init; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<ReferralLink> ReferralLinks { get; set; }
    public DbSet<User> Users { get; set; }
    

    public ReferralsContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    
}