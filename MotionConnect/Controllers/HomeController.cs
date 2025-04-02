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
            if (!User.Identity.IsAuthenticated)
                return View(null);

            string identifier = User.Identity.Name;
            var anvandare = await _userManger.FindByEmailAsync(identifier) ?? await _userManger.FindByNameAsync(identifier);

            var notiser = await _context.Notiser
            .CountAsync(n => n.AnvandarId == anvandare.Id);

            var meddelanden = await _context.Meddelanden
                .Include(m => m.Mottagare)
                .Include(m => m.Avsandare)
                .Where(m => m.AvsandareId == anvandare.Id || m.Mottagare.Any(r => r.MottagareId == anvandare.Id))
                .ToListAsync();

            var chattaMed = new List<ApplicationUser>();

            foreach (var m in meddelanden)
            {
                foreach (var mottagare in m.Mottagare)
                {
                    if(mottagare.MottagareId != anvandare.Id)
                    {
                        var anv = await _userManger.FindByIdAsync(mottagare.MottagareId);
                        if (anv != null && !chattaMed.Any(a => a.Id == anv.Id))
                        chattaMed.Add(anv);
                    }
                }

                if(m.AvsandareId != anvandare.Id)
                {
                    var avsandare = await _userManger.FindByIdAsync(m.AvsandareId);
                    if (avsandare != null && !chattaMed.Any(a => a.Id == avsandare.Id))
                        chattaMed.Add(anvandare);
                }
            }

            ViewBag.Chattar = chattaMed;

            ViewBag.Notiser = notiser;
                            
            return View(anvandare);
        }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
