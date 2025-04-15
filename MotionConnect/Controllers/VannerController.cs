using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class VannerController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    // Konstruktor som kopplar in användarhantering, inloggning och databas
    public VannerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // POST: Lägger till en vän och skapar en notis till den andra personen
    [HttpPost]
    public async Task<IActionResult> laggTillVan(string id)
    {
        var userId = _userManager.GetUserId(User);
        var anvandare = await _userManager.FindByIdAsync(userId);

        // Om användaren inte är inloggad skickas man till startsidan
        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Skapa vänrelation
        var vanner = new Van
        {
            AnvandarId = anvandare.Id,
            VanId = id
        };

        // Skapa en notis till personen som blev tillagd
        var notis = new Notis
        {
            Meddelande = $"{anvandare.ForNamn} {anvandare.EfterNamn} har blivit din vän",
            Typ = NotisTyp.Annat,
            ArLast = false,
            AnvandarId = id, // Den som får notisen
            SkapadesTid = DateTime.UtcNow
        };

        // Spara vänskapen och notisen i databasen
        _context.Vanner.Add(vanner);
        _context.Notiser.Add(notis);
        await _context.SaveChangesAsync();

        // Skicka tillbaka till profilvyn för den man blev vän med
        return RedirectToAction("KontoInfo", "Konto", new { id = id });
    }

    // POST: Tar bort en vänskapsrelation om den finns
    [HttpPost]
    public async Task<IActionResult> taBortVan(string id)
    {
        var userId = _userManager.GetUserId(User);

        // Hitta vänrelationen i båda riktningar
        var vanrelation = await _context.Vanner
            .FirstOrDefaultAsync(v =>
                (v.AnvandarId == userId && v.VanId == id) ||
                (v.AnvandarId == id && v.VanId == userId));

        // Om användaren inte är inloggad, gå tillbaka till startsidan
        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Om relationen finns, ta bort den från databasen
        if (vanrelation != null)
        {
            _context.Vanner.Remove(vanrelation);
            await _context.SaveChangesAsync();
        }

        // Skicka tillbaka till profilen man just tagit bort
        return RedirectToAction("KontoInfo", "Konto", new { id = id });
    }
}
