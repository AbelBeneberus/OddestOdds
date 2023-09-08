using Microsoft.EntityFrameworkCore;
using OddestOdds.Data.Models;

namespace OddestOdds.Data.Database
{
    public class FixtureContext : DbContext
    {
        public DbSet<Fixture> Fixtures { get; set; } = null!;
        public DbSet<Market> Markets { get; set; } = null!;
        public DbSet<MarketSelection> MarketSelections { get; set; } = null!;

        public FixtureContext(DbContextOptions<FixtureContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fixture>(fixture =>
            {
                fixture.HasKey(f => f.Id);
                fixture.HasMany<Market>(f => f.Markets)
                    .WithOne()
                    .HasForeignKey(m => m.FixtureId);
            });

            modelBuilder.Entity<Market>(market =>
            {
                market.HasKey(m => m.Id);
                market.HasMany(m => m.Selections)
                    .WithOne()
                    .HasForeignKey(s => s.MarketId);
            });

            modelBuilder.Entity<MarketSelection>(selection =>
            {
                selection.HasKey(s => s.Id);
                selection.Property(s => s.Odd).HasColumnType("decimal(18,2)");
            });
        }
    }
}