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
    private readonly UserManager<ApplicationUser> _userManger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _logger = logger;
        _userManger = userManager;
        _signInManager = signInManager;
        _context = context;

    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManger.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        if (!User.Identity.IsAuthenticated)
            return View(new HomeViewModel());

        string identifier = User.Identity.Name;
        var anvandare = await _userManger.FindByEmailAsync(identifier) ?? await _userManger.FindByNameAsync(identifier);
        var anvandarId = _userManger.GetUserId(User);

        // 🔔 Notiser
        var notiser = await _context.Notiser
            .CountAsync(n => n.AnvandarId == anvandare.Id);

        // 💬 Chattar och motparter
        var chattar = await _context.Chattar
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Mottagare)
                    .ThenInclude(mm => mm.Mottagare)
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Avsandare)
            .Where(c => c.Meddelanden.Any(m =>
                m.AvsandareId == anvandarId || m.Mottagare.Any(r => r.MottagareId == anvandarId)))
            .ToListAsync();

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

        motparter = motparter.Distinct().ToList();

        // 📝 Inlägg (exakt som i VisaInlagg)
        var inlagg = await _context.Inlagg
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
                .ThenInclude(isport => isport.Sport)
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        var antalGillningarPerInlagg = await _context.Gillningar
            .GroupBy(g => g.InlaggId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        var harGillad = await _context.Gillningar
            .Where(g => g.AnvandarId == anvandarId)
            .Select(g => g.InlaggId)
            .ToListAsync();

        // ✅ ViewModel
        var model = new HomeViewModel
        {
            Anvandare = anvandare,
            Chattar = motparter,
            Inlagg = inlagg,
            HarGillatInlaggIds = harGillad,
            AntalGillningar = antalGillningarPerInlagg
        };

        ViewBag.Notiser = notiser;

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
