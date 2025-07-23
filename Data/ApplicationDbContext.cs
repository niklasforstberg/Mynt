using Microsoft.EntityFrameworkCore;
using Mynt.Models;
namespace Mynt.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add your DbSet properties here
    public DbSet<User> Users { get; set; }
    public DbSet<FinancialGroup> FinancialGroups { get; set; }
    public DbSet<FinancialGroupMember> FinancialGroupMembers { get; set; }
    public DbSet<FinancialGroupInvitation> FinancialGroupInvitations { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetValue> AssetValues { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<AssetTypeTranslation> AssetTypeTranslations { get; set; } = null!;
    public DbSet<Currency> Currencies { get; set; } = null!;
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Currency: Primary key configuration
        modelBuilder.Entity<Currency>()
            .HasKey(c => c.Code);

        // FinancialGroupMember: composite key
        modelBuilder.Entity<FinancialGroupMember>()
            .HasKey(fgm => new { fgm.UserId, fgm.FinancialGroupId });

        modelBuilder.Entity<FinancialGroupMember>()
            .HasOne(fgm => fgm.User)
            .WithMany(u => u.FinancialGroupMemberships)
            .HasForeignKey(fgm => fgm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FinancialGroupMember>()
            .HasOne(fgm => fgm.FinancialGroup)
            .WithMany(fg => fg.Members)
            .HasForeignKey(fgm => fgm.FinancialGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // FinancialGroupInvitation: relationships
        modelBuilder.Entity<FinancialGroupInvitation>()
            .HasOne(inv => inv.FinancialGroup)
            .WithMany(fg => fg.Invitations)
            .HasForeignKey(inv => inv.FinancialGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FinancialGroupInvitation>()
            .HasOne(inv => inv.InvitedByUser)
            .WithMany()  // No navigation property on User side
            .HasForeignKey(inv => inv.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserActivity: relationships
        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasOne(e => e.User)
                .WithMany()  // No navigation property on User side
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Asset: relationships
        modelBuilder.Entity<Asset>()
            .HasOne(a => a.FinancialGroup)
            .WithMany(fg => fg.Assets)
            .HasForeignKey(a => a.FinancialGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.User)
            .WithMany(u => u.Assets)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.AssetType)
            .WithMany()
            .HasForeignKey(a => a.AssetTypeId);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.Currency)
            .WithMany(c => c.Assets)
            .HasForeignKey(a => a.CurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        // AssetValue: relationship
        modelBuilder.Entity<AssetValue>()
            .HasOne(av => av.Asset)
            .WithMany(a => a.AssetValues)
            .HasForeignKey(av => av.AssetId);

        // Fix for AssetValue.Value decimal precision warning
        modelBuilder.Entity<AssetValue>()
            .Property(av => av.Value)
            .HasPrecision(18, 2);  // Adjust precision/scale as needed for your use case

        // Currency: relationships
        modelBuilder.Entity<Currency>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // CurrencyExchangeRate: relationships
        modelBuilder.Entity<CurrencyExchangeRate>()
            .HasOne(r => r.FromCurrency)
            .WithMany(c => c.ExchangeRates)
            .HasForeignKey(r => r.FromCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CurrencyExchangeRate>()
            .HasOne(r => r.ToCurrency)
            .WithMany()
            .HasForeignKey(r => r.ToCurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CurrencyExchangeRate>()
            .HasOne(r => r.SetBy)
            .WithMany()
            .HasForeignKey(r => r.SetById)
            .OnDelete(DeleteBehavior.Restrict);

        // Fix for CurrencyExchangeRate.Rate decimal precision
        modelBuilder.Entity<CurrencyExchangeRate>()
            .Property(r => r.Rate)
            .HasPrecision(18, 6);  // High precision for exchange rates
    }
}