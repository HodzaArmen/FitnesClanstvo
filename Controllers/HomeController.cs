using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitnesClanstvo.Models;
using FitnesClanstvo.Data;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FitnesClanstvo.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FitnesContext _context;

    public HomeController(ILogger<HomeController> logger, FitnesContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        var vadbe = await _context.Vadbe.ToListAsync();
        var clani = await _context.Clani.ToListAsync();
        // Get new members in the last month
        var newMembersCount = await _context.Clanstva
            .Where(c => c.Zacetek.Month == DateTime.Now.AddMonths(0).Month && c.Zacetek.Year == DateTime.Now.Year)
            .CountAsync();
        ViewBag.NewMembersCount = newMembersCount;
        // Get members whose membership is about to expire
        var upcomingNotifications = await _context.Clanstva
            .Where(c => c.Zacetek.AddYears(1) <= DateTime.Now.AddMonths(1))  // Example: Expiring in the next month
            .ToListAsync();

        ViewBag.UpcomingNotifications = upcomingNotifications; // Pass upcoming notifications to the view

        // Get monthly member statistics based on Clanstvo's start date (Zacetek)
        var membersPerMonth = await _context.Clanstva
            .GroupBy(c => new { c.Zacetek.Year, c.Zacetek.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MemberCount = g.Count()
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();

        var popularVadbe = await _context.Vadbe
            .Include(v => v.Rezervacije)  // Ensure reservations are loaded
            .OrderByDescending(v => v.Rezervacije.Count)
            .Take(5)  // Example: Top 5 most popular exercises
            .ToListAsync();

        // Get monthly income (sum of payments for each month)
        var incomePerMonth = await _context.Placila
            .GroupBy(p => new { p.Clanstvo.Zacetek.Year, p.Clanstvo.Zacetek.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Sum(p => p.Znesek) // Sum of payments for each month
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();

        // Calculate total income for the last month
        var totalIncome = incomePerMonth.LastOrDefault()?.Income ?? 0;

        ViewBag.Clani = clani;  // Pass the list of members to the view
        ViewBag.TotalIncome = totalIncome;  // Pass total income to the view
        ViewBag.PopularVadbe = popularVadbe;  // Pass popular exercises to the view

        // Fetch user reservations
        var userReservations = User.Identity.IsAuthenticated 
            ? await _context.Rezervacije
                .Where(r => r.Clan.Ime == User.Identity.Name && r.DatumRezervacije >= DateTime.Now)
                .ToListAsync() 
            : new List<Rezervacija>(); // Ensure it's never null

        ViewBag.UserReservations = userReservations;

        // Fetch the user membership details
        var user = await _context.Clani
            .FirstOrDefaultAsync(c => c.Ime == User.Identity.Name); // Assuming you store the username

        if (user != null)
        {
            var clanstvo = await _context.Clanstva.FirstOrDefaultAsync(c => c.ClanId == user.Id);

            if (clanstvo != null)
            {
                ViewBag.StartDate = clanstvo.Zacetek.ToString("dd.MM.yyyy"); // Membership start date
                // Determine membership status based on start and end dates
                ViewBag.MembershipStatus = (clanstvo.Konec >= DateTime.Now) ? "Active" : "Expired"; 
            }
            else
            {
                ViewBag.MembershipStatus = "No membership found";
            }
        }

        return View(new HomeIndexViewModel
        {
            Vadbe = vadbe,
            MonthlyStatistics = membersPerMonth.Select(m => new MonthlyStatistic
            {
                Year = m.Year,
                Month = m.Month,
                MemberCount = m.MemberCount
            }).ToList()
        });
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rezerviraj(int vadbaId, int izbraniClanId)
    {
        // Poiščemo člana v bazi
        var clan = await _context.Clani.FindAsync(izbraniClanId);
        if (clan == null)
        {
            return Content("Član ni bil najden.");
        }

        // Poiščemo vadbo v bazi
        var vadba = await _context.Vadbe.FindAsync(vadbaId);
        if (vadba == null)
        {
            return Content("Vadba ni bila najdena.");
        }

        // Ustvarimo novo rezervacijo
        var rezervacija = new Rezervacija
        {
            ClanId = izbraniClanId,
            VadbaId = vadbaId,
            DatumRezervacije = DateTime.Now // Nastavimo datum rezervacije na trenutni čas
        };

        // Dodamo rezervacijo v tabelo
        _context.Rezervacije.Add(rezervacija);
        await _context.SaveChangesAsync();

        // Preusmerimo uporabnika nazaj na začetno stran
        return RedirectToAction("Index");
    }

 

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}