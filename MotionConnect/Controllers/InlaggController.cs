using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;


public class InlaggController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public InlaggController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // Visar formulär för att skapa ett nytt inlägg
    [HttpGet]
    public async Task<IActionResult> SkapaEttInlagg()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        // Visar namn och profilbild i vyn om användaren är inloggad
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        // Hämtar alla sporter till en dropdown eller checkbox-lista
        var sporter = await _context.Sporter.ToListAsync();
        ViewBag.Sporter = sporter;

        return View();
    }

    // Visar alla inlägg på plattformen
    [HttpGet]
    public async Task<IActionResult> VisaInlagg()
    {
        // Lägger till användarnamn och bild i vyn om inloggad
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        var userId = _userManager.GetUserId(User);

        // Hämtar inlägg med tillhörande användare och sporter
        var inlagg = await _context.Inlagg
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
                .ThenInclude(isport => isport.Sport)
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        // Hämtar antal gillningar per inlägg
        var antalGillningarPerInlagg = await _context.Gillningar
            .GroupBy(g => g.InlaggId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        // Hämtar vilka inlägg den inloggade användaren har gillat
        var harGillad = await _context.Gillningar
            .Where(g => g.AnvandarId == userId)
            .Select(g => g.InlaggId)
            .ToListAsync();

        ViewBag.HarGillad = harGillad;
        ViewBag.AntalGillningar = antalGillningarPerInlagg;

        return View(inlagg ?? new List<Inlagg>());
    }

    // Visar inlägg som är skapade av den inloggade användaren
    [HttpGet]
    public async Task<IActionResult> VisaInlaggAnvandare()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("LoggaInPaKonto", "Konto");
        }

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        var userId = _userManager.GetUserId(User);

        var inlagg = await _context.Inlagg
            .Where(i => i.AnvandarId == userId)
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
                .ThenInclude(isport => isport.Sport)
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        var antalGillningarPerInlagg = await _context.Gillningar
            .GroupBy(g => g.InlaggId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        ViewBag.AntalGillningar = antalGillningarPerInlagg;

        return View(inlagg);
    }

    // Skapar ett nytt inlägg och sparar bild och koppling till sporter
    [HttpPost]
    public async Task<IActionResult> SkapaEttInlagg(string text, IFormFile bild, List<int> sportIds)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        var anvandare = await _userManager.FindByEmailAsync(User.Identity.Name);
        if (anvandare == null)
        {
            return NotFound("Användaren hittades inte");
        }

        // Hanterar uppladdning av bild om det finns en
        string bildUrl = null;
        if (bild != null && bild.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bild.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bild.CopyToAsync(stream);
            }

            bildUrl = "/uploads/" + fileName;
        }

        // Skapar själva inlägget
        var nyttInlagg = new Inlagg
        {
            Text = text,
            BildUrl = bildUrl,
            SkapadesTid = DateTime.UtcNow,
            Anvandare = anvandare,
            AnvandarId = anvandare.Id
        };

        _context.Inlagg.Add(nyttInlagg);
        await _context.SaveChangesAsync();

        // Länkar inlägget till valda sporter
        foreach (var sportId in sportIds)
        {
            var inlaggSport = new InlaggSport
            {
                InlaggId = nyttInlagg.InlaggId,
                SportId = sportId
            };
            _context.InlaggSporter.Add(inlaggSport);
        }
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    // Tar bort ett inlägg från databasen
    [HttpPost]
    public async Task<IActionResult> taBortInlagg(int id)
    {
        var inlaggId = await _context.Inlagg.FindAsync(id);

        if (inlaggId == null)
        {
            return NotFound();
        }

        _context.Inlagg.Remove(inlaggId);
        await _context.SaveChangesAsync();

        return RedirectToAction("VisaInlaggAnvandare", "Inlagg");
    }

    // Hanterar när en användare gillar ett inlägg
    [HttpPost]
    public async Task<IActionResult> GillaInlagg(int id)
    {
        var inlagg = await _context.Inlagg
            .Include(i => i.Anvandare)
            .FirstOrDefaultAsync(i => i.InlaggId == id);

        var anvandareId = _userManager.GetUserId(User);

        if (inlagg == null)
        {
            return NotFound();
        }

        var anvandare = await _userManager.FindByIdAsync(anvandareId);

        var gillning = new Gillning
        {
            InlaggId = inlagg.InlaggId,
            AnvandarId = anvandareId
        };

        // Skapar en notis till den som äger inlägget
        var notis = new Notis
        {
            Meddelande = $"{anvandare.ForNamn} {anvandare.EfterNamn} har gillad dit inlägg",
            Typ = NotisTyp.Gillning,
            ArLast = false,
            AnvandarId = inlagg.AnvandarId,
            SkapadesTid = DateTime.UtcNow,
            InlaggId = id
        };

        _context.Gillningar.Add(gillning);
        _context.Notiser.Add(notis);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    // Tar bort gillning och tillhörande notis
    [HttpPost]
    public async Task<IActionResult> taBortGillning(int id)
    {
        var inlagg = await _context.Inlagg.FindAsync(id);
        var anvandareId = _userManager.GetUserId(User);

        var gillning = await _context.Gillningar
            .FirstOrDefaultAsync(g => g.AnvandarId == anvandareId && g.InlaggId == id);

        var notis = await _context.Notiser
            .FirstOrDefaultAsync(n => n.AnvandarId == inlagg.AnvandarId && n.InlaggId == id && n.Typ == NotisTyp.Gillning);

        if (gillning != null)
            _context.Gillningar.Remove(gillning);

        if (notis != null)
            _context.Notiser.Remove(notis);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}
