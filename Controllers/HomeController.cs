using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MotionConnect.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace MotionConnect.Controllers;

public class HomeController : Controller
{
    // Identity- och databas-tjänster som injiceras
    private readonly UserManager<ApplicationUser> _userManger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    // Logger används vid behov för felsökning eller loggning
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _logger = logger;
        _userManger = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // Enkel metod för att visa "Privacy"-sidan
    public IActionResult Privacy()
    {
        return View();
    }

    // Huvudmetod för startsidan (Index)
    public async Task<IActionResult> Index(int? sportId)
    {
        // Om användaren är inloggad, skicka med namn och profilbild till vyn
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManger.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        // Om ingen är inloggad, visa en tom startsida
        if (!User.Identity.IsAuthenticated)
            return View(new HomeViewModel());

        // Hämta info om inloggad användare
        string identifier = User.Identity.Name;
        var anvandare = await _userManger.FindByEmailAsync(identifier) ?? await _userManger.FindByNameAsync(identifier);
        var anvandarId = _userManger.GetUserId(User);

        // Hämta antal notiser för den inloggade användaren
        var notiser = await _context.Notiser
            .CountAsync(n => n.AnvandarId == anvandare.Id);

        ViewBag.Notiser = notiser;

        // Hämta alla chattar där användaren är avsändare eller mottagare
        var chattar = await _context.Chattar
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Mottagare)
                    .ThenInclude(mm => mm.Mottagare)
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Avsandare)
            .Where(c => c.Meddelanden.Any(m =>
                m.AvsandareId == anvandarId || m.Mottagare.Any(r => r.MottagareId == anvandarId)))
            .ToListAsync();

        var grupper = await _context.GruppMedlemmar
            .Where(gm => gm.AnvandarId == anvandare.Id)
            .Select(gm => gm.Grupp)
            .ToListAsync();

        ViewBag.Grupper = grupper;

        // Lista på andra användare som den inloggade har chattat med
        var motparter = new List<ApplicationUser>();
        foreach (var chatt in chattar)
        {
            var andraPersoner = chatt.Meddelanden
                .Select(m => m.Avsandare)
                .Concat(chatt.Meddelanden.SelectMany(m => m.Mottagare.Select(r => r.Mottagare)))
                .Where(p => p.Id != anvandarId)
                .Distinct()
                .ToList();

            motparter.AddRange(andraPersoner);
        }

        // Ta bort dubbletter från listan
        motparter = motparter.Distinct().ToList();

        // Hämta alla inlägg, med användare och sporter kopplade
        var inlaggQuery = _context.Inlagg
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
                .ThenInclude(isport => isport.Sport)
            .AsQueryable();

        if (sportId.HasValue)
        {
            inlaggQuery = inlaggQuery
                .Where(i => i.InlaggSporter.Any(s => s.SportId == sportId));
        }

        var inlagg = await inlaggQuery
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        ViewBag.ValdSportId = sportId;

        var sporterMedInlagg = await _context.InlaggSporter
            .Include(i => i.Sport)
            .GroupBy(i => i.SportId)
            .Select(i => i.First().Sport)
            .ToListAsync();

        ViewBag.Sporter = sporterMedInlagg;

        // Räkna hur många gillningar varje inlägg har
        var antalGillningarPerInlagg = await _context.Gillningar
            .GroupBy(g => g.InlaggId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        // Hämta lista på inlägg som den inloggade användaren har gillat
        var harGillad = await _context.Gillningar
            .Where(g => g.AnvandarId == anvandarId)
            .Select(g => g.InlaggId)
            .ToListAsync();
        
        var antalKommentarePerInlagg = await _context.Kommentarer
            .GroupBy(k => k.InlaggId)
            .ToDictionaryAsync(k => k.Key, k => k.Count());

        // Skapa och fyll i ViewModel med all data
        var model = new HomeViewModel
        {
            Anvandare = anvandare,
            Chattar = motparter,
            Inlagg = inlagg,
            HarGillatInlaggIds = harGillad,
            AntalGillningar = antalGillningarPerInlagg,
            AntalKommentarer = antalKommentarePerInlagg
        };

        // Skicka med antalet notiser till vyn
        ViewBag.Notiser = notiser;

        return View(model);
    }

    // Standardfelhanterare (om något oväntat händer)
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
