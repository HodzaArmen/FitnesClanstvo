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
        var vadbe = await _context.Vadbe
            .Where(v => v.DatumInUra >= DateTime.Now) // Filtriramo le prihodnje vadbe
            .ToListAsync();
        var clani = await _context.Clani.ToListAsync();
        // Get new members in the last month
        var newMembersCount = await _context.Clanstva
            .Where(c => c.Zacetek.Month == DateTime.Now.AddMonths(0).Month && c.Zacetek.Year == DateTime.Now.Year)
            .CountAsync();
        ViewBag.NewMembersCount = newMembersCount;

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

        var userReservations = new List<Rezervacija>();
        if (User.Identity.IsAuthenticated)
        {
            userReservations = await _context.Rezervacije
                .Include(r => r.Vadba)
                .Where(r => r.Clan.Email == User.Identity.Name && r.DatumRezervacije >= DateTime.Now)
                .ToListAsync();
        }
        ViewBag.UserReservations = userReservations;
        var userReservedVadbeIds = userReservations.Select(r => r.VadbaId).ToList();
        ViewBag.UserReservedVadbeIds = userReservedVadbeIds;  // Shranimo ID-je vadb, za katere je uporabnik že rezerviral
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
        //ViewBag.UserReservations = userReservations;

        return View(new HomeIndexViewModel
        {
            Vadbe = vadbe,
            UserReservedVadbeIds = userReservedVadbeIds,
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
    public IActionResult Rezerviraj(int vadbaId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            _logger.LogWarning("User is not authenticated.");
            return RedirectToAction("Login", "Account");
        }

        var uporabnik = _context.Clani.SingleOrDefault(c => c.Email == User.Identity.Name);

        if (uporabnik == null)
        {
            _logger.LogError("Član ni bil najden.");
            TempData["ErrorMessage"] = "Član ni bil najden.";
            return RedirectToAction("Index", "Home");
        }

        var vadba = _context.Vadbe.Include(v => v.Rezervacije)
                                .SingleOrDefault(v => v.Id == vadbaId);

        if (vadba == null)
        {
            _logger.LogError("Vadba ni bila najdena.");
            TempData["ErrorMessage"] = "Vadba ni bila najdena.";
            return RedirectToAction("Index", "Home");
        }

        if (vadba.Kapaciteta <= vadba.Rezervacije.Count)
        {
            _logger.LogWarning("Vadba je že polna.");
            TempData["ErrorMessage"] = "Vadba je že polna.";
            return RedirectToAction("Index", "Home");
        }

        // Preverite, ali uporabnik že ima prihodnjo rezervacijo za to vadbo
        var obstojecaRezervacija = _context.Rezervacije
            .FirstOrDefault(r => r.ClanId == uporabnik.Id && r.VadbaId == vadba.Id && r.DatumRezervacije >= DateTime.Now);

        if (obstojecaRezervacija != null)
        {
            _logger.LogWarning("Že ste rezervirali to vadbo.");
            TempData["ErrorMessage"] = "Že ste rezervirali to vadbo.";
            return RedirectToAction("Index", "Home");
        }

        var rezervacija = new Rezervacija
        {
            VadbaId = vadba.Id,
            ClanId = uporabnik.Id,
            DatumRezervacije = vadba.DatumInUra // Set to the date and time of the exercise
        };

        _context.Rezervacije.Add(rezervacija);
        _context.SaveChanges();

        _logger.LogInformation("Rezervacija je bila uspešno narejena.");
        TempData["SuccessMessage"] = "Rezervacija je bila uspešno narejena!";
        return RedirectToAction("Index", "Home");
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