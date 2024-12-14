using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FitnesClanstvo.Models;
using FitnesClanstvo.Data;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.ViewModels;

namespace FitnesClanstvo.Controllers;

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

        ViewBag.Clani = clani; // Posreduj člane v pogled

        return View(new HomeIndexViewModel
        {
            Vadbe = vadbe
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
