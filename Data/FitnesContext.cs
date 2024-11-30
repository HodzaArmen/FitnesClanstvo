using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Models;

namespace FitnesClanstvo.Data;
public class FitnesContext : DbContext
{
    public FitnesContext(DbContextOptions<FitnesContext> options) : base(options) { }

    public DbSet<Clan>? Clani { get; set; }
    public DbSet<Clanstvo>? Clanstva { get; set; }
    public DbSet<Vadba>? Vadbe { get; set; }
    public DbSet<Rezervacija>? Rezervacije { get; set; }
    public DbSet<Prisotnost>? Prisotnosti { get; set; }
    public DbSet<Placilo>? Placila { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Placilo>()
            .Property(p => p.Znesek)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}