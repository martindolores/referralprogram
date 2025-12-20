using Microsoft.EntityFrameworkCore;
using ReferralProgram.Servercore.Models;

namespace ReferralProgram.Servercore.Data;

public class ReferralDbContext : DbContext
{
    public ReferralDbContext(DbContextOptions<ReferralDbContext> options) : base(options)
    {
    }

    public DbSet<Referral> Referrals => Set<Referral>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Referral>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasIndex(r => r.ReferralCode).IsUnique();
            entity.HasIndex(r => r.PhoneNumber).IsUnique();
            entity.Property(r => r.ReferrerName).IsRequired().HasMaxLength(100);
            entity.Property(r => r.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(r => r.ReferralCode).IsRequired().HasMaxLength(20);
        });
    }
}
