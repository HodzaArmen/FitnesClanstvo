using FitnesClanstvo.Models;
using FitnesClanstvo.Data;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace FitnesClanstvo.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(FitnesContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            if (context.Clani.Any())
            {
                return; // DB is already initialized
            }

            // Add members
            var clani = new Clan[]
            {
                new Clan{Ime="Janez", Priimek="Novak", DatumRojstva=DateTime.Parse("1990-01-01"), Email="janez.novak@example.com"},
                new Clan{Ime="Maja", Priimek="Kovač", DatumRojstva=DateTime.Parse("1985-05-15"), Email="maja.kovac@example.com"},
                new Clan{Ime="Peter", Priimek="Horvat", DatumRojstva=DateTime.Parse("1992-03-20"), Email="peter.horvat@example.com"},
                new Clan{Ime="Anja", Priimek="Vidmar", DatumRojstva=DateTime.Parse("1994-02-10"), Email="anja.vidmar@example.com"},
                new Clan{Ime="Luka", Priimek="Breznik", DatumRojstva=DateTime.Parse("1988-07-07"), Email="luka.breznik@example.com"},
                new Clan{Ime="Eva", Priimek="Zupan", DatumRojstva=DateTime.Parse("1991-12-12"), Email="eva.zupan@example.com"},
                new Clan{Ime="Tina", Priimek="Oman", DatumRojstva=DateTime.Parse("1987-03-03"), Email="tina.oman@example.com"},
                new Clan{Ime="Marko", Priimek="Hrovat", DatumRojstva=DateTime.Parse("1993-11-22"), Email="marko.hrovat@example.com"},
                new Clan{Ime="Nina", Priimek="Potočnik", DatumRojstva=DateTime.Parse("1990-08-18"), Email="nina.potocnik@example.com"},
                new Clan{Ime="Matej", Priimek="Gradišnik", DatumRojstva=DateTime.Parse("1986-06-06"), Email="matej.gradisnik@example.com"}
            };
            context.Clani.AddRange(clani);
            context.SaveChanges();
            // Add activities
            var vadbe = new Vadba[]
            {
                new Vadba{Ime="Joga", DatumInUra=DateTime.Now.AddHours(1), Kapaciteta=20},
                new Vadba{Ime="Zumba", DatumInUra=DateTime.Now.AddHours(2), Kapaciteta=15},
                new Vadba{Ime="Pilates", DatumInUra=DateTime.Now.AddHours(3), Kapaciteta=10},
                new Vadba{Ime="Kardio", DatumInUra=DateTime.Now.AddHours(4), Kapaciteta=25},
                new Vadba{Ime="BodyPump", DatumInUra=DateTime.Now.AddHours(5), Kapaciteta=18},
                new Vadba{Ime="HIIT", DatumInUra=DateTime.Now.AddHours(6), Kapaciteta=12},
                new Vadba{Ime="Spinning", DatumInUra=DateTime.Now.AddHours(7), Kapaciteta=16},
                new Vadba{Ime="CrossFit", DatumInUra=DateTime.Now.AddHours(8), Kapaciteta=14},
                new Vadba{Ime="Aerobika", DatumInUra=DateTime.Now.AddHours(9), Kapaciteta=20},
                new Vadba{Ime="Bootcamp", DatumInUra=DateTime.Now.AddHours(10), Kapaciteta=15}
            };
            context.Vadbe.AddRange(vadbe);
            context.SaveChanges();

            // Add memberships
            var clanstva = clani.Select((clan, index) => new Clanstvo
            {
                Tip = index % 3 == 0 ? "Osnovno" : index % 3 == 1 ? "Premium" : "VIP",
                Cena = index % 3 == 0 ? 30.00m : index % 3 == 1 ? 50.00m : 70.00m,
                Zacetek = DateTime.Now,
                Konec = DateTime.Now.AddMonths(1),
                ClanId = clan.Id
            }).ToArray();
            context.Clanstva.AddRange(clanstva);
            context.SaveChanges();

            // Add reservations
            var rezervacije = clani.Select((clan, index) => new Rezervacija
            {
                DatumRezervacije = DateTime.Now,
                ClanId = clan.Id,
                VadbaId = vadbe[index % vadbe.Length].Id
            }).ToArray();
            context.Rezervacije.AddRange(rezervacije);
            context.SaveChanges();

            // Add payments
            var placila = clanstva.Select(clanstvo => new Placilo
            {
                DatumPlacila = DateTime.Now,
                Znesek = clanstvo.Cena,
                ClanstvoId = clanstvo.Id
            }).ToArray();
            context.Placila.AddRange(placila);
            context.SaveChanges();

            // Add attendance
            var prisotnosti = clani.Select((clan, index) => new Prisotnost
            {
                DatumPrisotnosti = DateTime.Now,
                ClanId = clan.Id,
                VadbaId = vadbe[index % vadbe.Length].Id
            }).ToArray();
            context.Prisotnosti.AddRange(prisotnosti);
            context.SaveChanges();

            // // Add roles
            // var roles = new IdentityRole[]
            // {
            //     new IdentityRole{Name="Administrator"},
            //     new IdentityRole{Name="Manager"},
            //     new IdentityRole{Name="User"}
            // };
            // context.Roles.AddRange(roles);
            // context.SaveChanges();

            // // Add users
            // var users = clani.Select(clan => new ApplicationUser
            // {
            //     FirstName = clan.Ime,
            //     LastName = clan.Priimek,
            //     Email = clan.Email,
            //     UserName = clan.Email,
            //     EmailConfirmed = true
            // }).ToList();

            // users.Insert(0, new ApplicationUser{FirstName="Admin", LastName="User", Email="admin@example.com", UserName="admin@example.com", NormalizedEmail = "ADMIN@EXAMPLE.COM", NormalizedUserName = "ADMIN@EXAMPLE.COM", EmailConfirmed=true});
            // users.Insert(1, new ApplicationUser{FirstName="Manager1", LastName="User", Email="manager1@example.com", UserName="manager1@example.com", NormalizedEmail = "MANAGAR1@EXAMPLE.COM", NormalizedUserName = "MANAGAR1@EXAMPLE.COM", EmailConfirmed=true});
            // users.Insert(2, new ApplicationUser{FirstName="Manager2", LastName="User", Email="manager2@example.com", UserName="manager2@example.com", NormalizedEmail = "MANAGAR2@EXAMPLE.COM", NormalizedUserName = "MANAGAR2@EXAMPLE.COM", EmailConfirmed=true});

            // var passwordHasher = new PasswordHasher<ApplicationUser>();
            // foreach (var user in users)
            // {
            //     user.PasswordHash = passwordHasher.HashPassword(user, "Password1!");
            //     context.Users.Add(user);
            // }
            // context.SaveChanges();

            // // Assign roles
            // var userRoles = new List<IdentityUserRole<string>>
            // {
            //     new IdentityUserRole<string> { RoleId = roles[0].Id, UserId = users[0].Id }, // Admin
            //     new IdentityUserRole<string> { RoleId = roles[1].Id, UserId = users[1].Id }, // Manager 1
            //     new IdentityUserRole<string> { RoleId = roles[1].Id, UserId = users[2].Id }  // Manager 2
            // };

            // userRoles.AddRange(users.Skip(3).Select(user => new IdentityUserRole<string>
            // {
            //     RoleId = roles[2].Id,
            //     UserId = user.Id
            // }));

            // context.UserRoles.AddRange(userRoles);
            // context.SaveChanges();
            // foreach (var clan in clani)
            // {
            //     var user = new ApplicationUser
            //     {
            //         FirstName = clan.Ime,
            //         LastName = clan.Priimek,
            //         Email = clan.Email,
            //         UserName = clan.Email,
            //         EmailConfirmed = true
            //     };

            //     var result = await userManager.CreateAsync(user, "Password1!");

            //     if (result.Succeeded)
            //     {
            //         await userManager.AddToRoleAsync(user, "User");
            //     }
            // }
            // Add roles
            string[] roleNames = { "Administrator", "Manager", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Add users and assign roles
            var defaultPassword = "Password1!";
            foreach (var clan in clani)
            {
                var user = new ApplicationUser
                {
                    FirstName = clan.Ime,
                    LastName = clan.Priimek,
                    Email = clan.Email,
                    UserName = clan.Email,
                    EmailConfirmed = true
                };

                var userExists = await userManager.FindByEmailAsync(user.Email);
                if (userExists == null)
                {
                    var result = await userManager.CreateAsync(user, defaultPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }

            // Add an admin user
            var adminUser = new ApplicationUser
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                UserName = "admin@example.com",
                EmailConfirmed = true
            };

            var adminExists = await userManager.FindByEmailAsync(adminUser.Email);
            if (adminExists == null)
            {
                var result = await userManager.CreateAsync(adminUser, defaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }
        }
    }
}