using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Models;

namespace FitnesClanstvo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FitnesClanstvo.Models.Clan> Clan { get; set; } = default!;
        public DbSet<FitnesClanstvo.Models.Clanstvo> Clanstvo { get; set; } = default!;
        public DbSet<FitnesClanstvo.Models.Placilo> Placilo { get; set; } = default!;
        public DbSet<FitnesClanstvo.Models.Prisotnost> Prisotnost { get; set; } = default!;
        public DbSet<FitnesClanstvo.Models.Vadba> Vadba { get; set; } = default!;
        public DbSet<FitnesClanstvo.Models.Rezervacija> Rezervacija { get; set; } = default!;
    }
}
