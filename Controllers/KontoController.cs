using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class KontoController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    // Konstruktor för att koppla in användarhantering, inloggning och databas
    public KontoController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // GET: Visa sidan för att skapa konto
    [HttpGet]
    public async Task<IActionResult> SkapaKonto()
    {
        // Hämtar alla sporter från databasen och skickar till vyn via ViewBag
        var sporter = await _context.Sporter.ToListAsync();
        ViewBag.Sporter = sporter;
        return View();
    }

    // GET: Visa inloggningssidan
    [HttpGet]
    public IActionResult LoggaInPaKontot()
    {
        return View();
    }

    // GET: Visar inloggad användares kontoinfo
    [HttpGet]
    public async Task<IActionResult> KontoInfoAnvandare()
    {
        // Om ingen är inloggad, skicka till inloggningen
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("LoggaIn", "Konto");

        // Hämta användarens namn och profilbild för visning
        var user = await _userManager.GetUserAsync(User);
        ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        ViewBag.ProfilBild = user.ProfilBildUrl;

        // Hämtar användaren med dess sporter och vänner
        var anvandare = await _userManager.Users
            .Include(a => a.AnvandareSporter).ThenInclude(a => a.Sport)
            .Include(a => a.Vanner).ThenInclude(a => a.VanAnvandare)
            .FirstOrDefaultAsync(a => a.Email == User.Identity.Name);
        
        var userId = _userManager.GetUserId(User);
        var inlagg = await _context.Inlagg
            .Where(i => i.AnvandarId == userId)
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
                .ThenInclude(isport => isport.Sport)
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();
        ViewBag.Inlagg = inlagg;

        // Räknar antalet vänner
        var antalVanner = await _context.Vanner
            .CountAsync(v => v.AnvandarId == anvandare.Id || v.VanId == anvandare.Id);

        var antalGillningarPerInlagg = await _context.Gillningar
            .GroupBy(g => g.InlaggId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        ViewBag.AntalVanner = antalVanner;
        ViewBag.AntalLikes = antalGillningarPerInlagg;
        return View(anvandare);
    }

    // GET: Visar info om en annan användares profil
    [HttpGet]
    public async Task<IActionResult> KontoInfo(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        ViewBag.ProfilBild = user.ProfilBildUrl;

        var inloggadId = _userManager.GetUserId(User);

        // Hämtar profilen man klickat på
        var anvandare = await _userManager.Users
            .Include(u => u.AnvandareSporter).ThenInclude(asport => asport.Sport)
            .FirstOrDefaultAsync(u => u.Id == id);

        // Kollar om man är vän med användaren
        bool arVanner = await _context.Vanner
            .AnyAsync(v => (v.AnvandarId == inloggadId && v.VanId == id) ||
                           (v.AnvandarId == id && v.VanId == inloggadId));

        if (anvandare == null) return NotFound();
        ViewBag.ArVanner = arVanner;

        // Räknar antalet vänner
        var antalVanner = await _context.Vanner
            .CountAsync(v => v.AnvandarId == anvandare.Id || v.VanId == anvandare.Id);

        ViewBag.AntalVanner = antalVanner;
        return View(anvandare);
    }

    // GET: Live-sökning av användare via för- eller efternamn
    [HttpGet]
    public async Task<IActionResult> SokKonto(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Json(new { });

        var resultat = await _context.Users
            .Where(u => u.ForNamn.Contains(query) || u.EfterNamn.Contains(query))
            .Select(u => new { u.Id, u.ForNamn, u.EfterNamn })
            .ToListAsync();

        return Json(resultat);
    }

    // GET: Visar inloggad användares notiser
    [HttpGet]
    public async Task<IActionResult> VisaNotiser()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.Profilbild = user.ProfilBildUrl;
        }

        var userId = _userManager.GetUserId(User);

        var notiser = await _context.Notiser
            .Include(n => n.Avsandare)
            .Where(n => n.AnvandarId == userId)
            .OrderByDescending(n => n.SkapadesTid)
            .ToListAsync();

        return View(notiser);
    }

    // POST: Byt profilbild för inloggad användare
    [HttpPost]
    public async Task<IActionResult> KontoInfoAnvandare(IFormFile bild)
    {
        var userId = _userManager.GetUserId(User);
        var anvandare = await _userManager.FindByIdAsync(userId);

        if (bild != null && bild.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Ta bort gammal bild om den inte är default
            if (!string.IsNullOrEmpty(anvandare.ProfilBildUrl) && !anvandare.ProfilBildUrl.Contains("default.png"))
            {
                var gammalBildSökväg = Path.Combine(uploadsFolder, Path.GetFileName(anvandare.ProfilBildUrl));
                if (System.IO.File.Exists(gammalBildSökväg))
                    System.IO.File.Delete(gammalBildSökväg);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bild.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await bild.CopyToAsync(stream);
            }

            anvandare.ProfilBildUrl = "/uploads/" + fileName;
            await _userManager.UpdateAsync(anvandare);
        }

        return RedirectToAction("KontoInfoAnvandare", "Konto");
    }

    // POST: Växla mellan öppen eller privat profil
    [HttpPost]
    public async Task<IActionResult> KontoInfoAnvandareAndraLagge()
    {
        var userId = _userManager.GetUserId(User);
        var anvandare = await _userManager.FindByIdAsync(userId);

        anvandare.ArProfilOppen = !anvandare.ArProfilOppen;
        await _userManager.UpdateAsync(anvandare);

        return RedirectToAction("KontoInfoAnvandare", "Konto");
    }

    // POST: Skapa nytt konto och ladda upp profilbild
    [HttpPost]
    public async Task<IActionResult> SkapaKonto(RegisterViewModel model, List<int> sportIds)
    {
        if (!ModelState.IsValid) return View(model);

        var anvandare = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.Telefonnummer,
            ForNamn = model.Fornamn,
            EfterNamn = model.Efternamn,
            FodelsAr = model.Fodelsear,
            ArProfilOppen = model.ArProfilOppen,
            ProfilBildUrl = "/uploads/defualt.png"
        };

        if (model.Profilbild != null && model.Profilbild.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var filePath = Path.Combine(uploadsFolder, model.Profilbild.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Profilbild.CopyToAsync(stream);
            }

            anvandare.ProfilBildUrl = "/uploads/" + model.Profilbild.FileName;
        }

        var result = await _userManager.CreateAsync(anvandare, model.Losenord);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // Länka sportintressen till användaren
        foreach (var sportId in sportIds)
        {
            _context.AnvandareSporter.Add(new AnvandareSport { AnvandarId = anvandare.Id, SportId = sportId });
        }
        await _context.SaveChangesAsync();

        await _signInManager.SignInAsync(anvandare, isPersistent: false);
        return RedirectToAction("Index", "Home");
    }

    // POST: Logga in på befintligt konto
    [HttpPost]
    public async Task<IActionResult> LoggaInPaKontot(string email, string losenord)
    {
        var anvandare = await _userManager.FindByEmailAsync(email);
        if (anvandare == null)
        {
            ModelState.AddModelError("", "Användaren finns inte");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(email, losenord, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", "Lösenordet är fel eller tillhör inte användaren");
        return View();
    }

    // POST: Logga ut användare
    [HttpPost]
    public async Task<IActionResult> LoggaUtFranKonto()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

