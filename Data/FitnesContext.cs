using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FitnesClanstvo.Data;
public class FitnesContext : IdentityDbContext<ApplicationUser>
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
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Clan>().ToTable("Clan");
        modelBuilder.Entity<Clanstvo>().ToTable("Clanstvo");
        modelBuilder.Entity<Vadba>().ToTable("Vadba");
        modelBuilder.Entity<Rezervacija>().ToTable("Rezervacija");
        modelBuilder.Entity<Prisotnost>().ToTable("Prisotnost");
        modelBuilder.Entity<Placilo>().ToTable("Placilo");
    }
}