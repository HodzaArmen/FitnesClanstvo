using FitnesClanstvo.Models;
using FitnesClanstvo.Data;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace FitnesClanstvo.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(FitnesContext context)
        {
            context.Database.EnsureCreated();
            // Preveri, ali so člani že dodani.
            if (context.Clani.Any())
            {
                return;   // DB je že inicializiran
            }

            var clani = new Clan[]
            {
                new Clan{Ime="Janez", Priimek="Novak", DatumRojstva=DateTime.Parse("1990-01-01"), Email="janez.novak@example.com"},
                new Clan{Ime="Maja", Priimek="Kovač", DatumRojstva=DateTime.Parse("1985-05-15"), Email="maja.kovac@example.com"},
                new Clan{Ime="Peter", Priimek="Horvat", DatumRojstva=DateTime.Parse("1992-03-20"), Email="peter.horvat@example.com"}
            };
            context.Clani.AddRange(clani);
            context.SaveChanges();

            var vadbe = new Vadba[]
            {
                new Vadba{Ime="Joga", DatumInUra=DateTime.Now.AddHours(1), Kapaciteta=20},
                new Vadba{Ime="Zumba", DatumInUra=DateTime.Now.AddHours(2), Kapaciteta=15}
            };
            context.Vadbe.AddRange(vadbe);
            context.SaveChanges();

            var clanstva = new Clanstvo[]
            {
                new Clanstvo{Tip="Osnovno", Cena=30.00m, Zacetek=DateTime.Now, Konec=DateTime.Now.AddMonths(1), ClanId=1},
                new Clanstvo{Tip="Premium", Cena=50.00m, Zacetek=DateTime.Now, Konec=DateTime.Now.AddMonths(1), ClanId=2},
                new Clanstvo{Tip="VIP", Cena=70.00m, Zacetek=DateTime.Now, Konec=DateTime.Now.AddMonths(1), ClanId=3}
            };
            context.Clanstva.AddRange(clanstva);
            context.SaveChanges();

            var rezervacije = new Rezervacija[]
            {
                new Rezervacija{DatumRezervacije=DateTime.Now, ClanId=1, VadbaId=1},
                new Rezervacija{DatumRezervacije=DateTime.Now, ClanId=2, VadbaId=2},
                new Rezervacija{DatumRezervacije=DateTime.Now, ClanId=3, VadbaId=1}
            };
            context.Rezervacije.AddRange(rezervacije);
            context.SaveChanges();

            var placila = new Placilo[]
            {
                new Placilo{DatumPlacila=DateTime.Now, Znesek=30.00m, ClanstvoId=1},
                new Placilo{DatumPlacila=DateTime.Now, Znesek=50.00m, ClanstvoId=2},
                new Placilo{DatumPlacila=DateTime.Now, Znesek=70.00m, ClanstvoId=3}
            };
            context.Placila.AddRange(placila);
            context.SaveChanges();

            var prisotnosti = new Prisotnost[]
            {
                new Prisotnost{DatumPrisotnosti=DateTime.Now, ClanId=1, VadbaId=1},
                new Prisotnost{DatumPrisotnosti=DateTime.Now, ClanId=2, VadbaId=2},
                new Prisotnost{DatumPrisotnosti=DateTime.Now, ClanId=3, VadbaId=1}
            };
            context.Prisotnosti.AddRange(prisotnosti);
            context.SaveChanges();

            // Preveri in dodaj vloge, če še ne obstajajo
            var roles = new IdentityRole[] {
                new IdentityRole{Id="1", Name="Administrator"},
                new IdentityRole{Id="2", Name="Manager"},
                new IdentityRole{Id="3", Name="User"}
            };
            foreach (IdentityRole r in roles)
            {
                context.Roles.Add(r);
            }

            var user = new ApplicationUser
            {
                FirstName = "Bob",
                LastName = "Dilon",
                City = "Ljubljana",
                Email = "bob@example.com",
                NormalizedEmail = "XXXX@EXAMPLE.COM",
                UserName = "bob@example.com",
                NormalizedUserName = "bob@example.com",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user,"Testni123!");
                user.PasswordHash = hashed;
                context.Users.Add(user);
                
            }
            context.SaveChanges();

            var UserRoles = new IdentityUserRole<string>[]
            {
                new IdentityUserRole<string>{RoleId = roles[0].Id, UserId=user.Id},
                new IdentityUserRole<string>{RoleId = roles[1].Id, UserId=user.Id},
            };
            foreach (IdentityUserRole<string> r in UserRoles)
            {
                context.UserRoles.Add(r);
            }
            context.SaveChanges();
        }
    }
}
