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


    public KontoController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> SkapaKonto()
    {
        var sporter = await _context.Sporter.ToListAsync();
        ViewBag.Sporter = sporter;
        return View();
    }

    [HttpGet]
    public IActionResult LoggaInPaKontot()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> KontoInfoAnvandare()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("LoggaIn", "Konto");
        }

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        }


        var anvandare = await _userManager.Users
        .Include(a => a.AnvandareSporter)
            .ThenInclude(a => a.Sport)
        .Include(a => a.Vanner)
            .ThenInclude(a => a.VanAnvandare)
        .FirstOrDefaultAsync(a => a.Email == User.Identity.Name);

        var antalVanner = await _context.Vanner
            .CountAsync(v => v.AnvandarId == anvandare.Id || v.VanId == anvandare.Id);

        ViewBag.AntalVanner = antalVanner;
        return View(anvandare);
    }

    [HttpGet]
    public async Task<IActionResult> SokKonto(string query)
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        }


        if (string.IsNullOrWhiteSpace(query))
        {
            return Json(new { });
        }

        var resultat = await _context.Users
        .Where(u => u.ForNamn.Contains(query) || u.EfterNamn.Contains(query))
        .Select(u => new { u.Id, u.ForNamn, u.EfterNamn })
        .ToListAsync();

        return Json(resultat);
    }

    [HttpGet]
    public async Task<IActionResult> KontoInfo(string id)
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        }

        var inloggadId = _userManager.GetUserId(User);
        var anvandaren = await _userManager.FindByIdAsync(id);

        var anvandare = await _userManager.Users
            .Include(u => u.AnvandareSporter)
            .ThenInclude(asport => asport.Sport)
            .FirstOrDefaultAsync(u => u.Id == id);

        bool arVanner = await _context.Vanner
            .AnyAsync(v => (v.AnvandarId == inloggadId && v.VanId == id) ||
                            (v.AnvandarId == id && v.VanId == inloggadId));

        if (anvandare == null) return NotFound();

        ViewBag.ArVanner = arVanner;

        return View(anvandare);
    }

    [HttpGet]
    public async Task<IActionResult> VisaNotiser()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        }

        var userId = _userManager.GetUserId(User);

        var notiser = await _context.Notiser
        .Where(n => n.AnvandarId == userId)
        .OrderByDescending(n => n.SkapadesTid)
        .ToListAsync();

        return View(notiser);
    }



    [HttpPost]
    public async Task<IActionResult> KontoInfoAnvandare(IFormFile bild)
    {
        var userId = _userManager.GetUserId(User);
        var anvandare = await _userManager.FindByIdAsync(userId);

        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (bild != null && bild.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!string.IsNullOrEmpty(anvandare.ProfilBildUrl) && !anvandare.ProfilBildUrl.Contains("default.png"))
            {
                var gammalBildSökväg = Path.Combine(uploadsFolder, Path.GetFileName(anvandare.ProfilBildUrl));
                if (System.IO.File.Exists(gammalBildSökväg))
                {
                    System.IO.File.Delete(gammalBildSökväg);
                }
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

    [HttpPost]
    public async Task<IActionResult> KontoInfoAnvandareAndraLagge()
    {
        var userId = _userManager.GetUserId(User);
        var anvandare = await _userManager.FindByIdAsync(userId);

        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (anvandare.ArProfilOppen)
        {
            anvandare.ArProfilOppen = false;
        }
        else
        {
            anvandare.ArProfilOppen = true;
        }

        await _userManager.UpdateAsync(anvandare);

        return RedirectToAction("KontoInfoAnvandare", "Konto");
    }


    [HttpPost]
    public async Task<IActionResult> SkapaKonto(RegisterViewModel model, List<int> sportIds)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Profilbild == null || model.Profilbild.Length == 0)
        {
            model.Profilbild = null; // Ingen bild valdes
        }

        var anvandare = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.Telefonnummer,
            ForNamn = model.Fornamn,
            EfterNamn = model.Efternamn,
            FodelsAr = model.Fodelsear,
            ArProfilOppen = model.ArProfilOppen,
            ProfilBildUrl = model.Profilbild != null ? "/uploads/" + model.Profilbild.FileName : "/uploads/defualt.png"
        };

        if (model.Profilbild != null && model.Profilbild.Length > 0)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, model.Profilbild.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Profilbild.CopyToAsync(stream);
                }

                anvandare.ProfilBildUrl = "/uploads/" + model.Profilbild.FileName;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kunde inte spara profilbilden.");
                ModelState.AddModelError("", "Kunde inte spara profilbilden.");
                return View(model);
            }
        }

        var result = await _userManager.CreateAsync(anvandare, model.Losenord);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        foreach (var sportId in sportIds)
        {
            var anvandareSport = new AnvandareSport
            {
                AnvandarId = anvandare.Id,
                SportId = sportId
            };
            _context.AnvandareSporter.Add(anvandareSport);
        }
        await _context.SaveChangesAsync();
        // 🛠 Kolla om användaren loggas in
        Console.WriteLine("Hej");
        await _signInManager.SignInAsync(anvandare, isPersistent: false);
        Console.WriteLine("Hej");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> LoggaInPaKontot(string email, string losenord)
    {
        var anvandare = await _userManager.FindByEmailAsync(email);

        if (anvandare == null)
        {
            Console.WriteLine($"Anändaren med epost{email} finns inte");
            ModelState.AddModelError("", "Användaren finns inte");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(email, losenord, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            Console.WriteLine($"Användaren {email} är inloggad");
            return RedirectToAction("Index", "Home");
        }

        Console.WriteLine($"Inloggningen misslyckades för {email}");
        ModelState.AddModelError("", "Lösenordet är fel eller tillhör inte användaren");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoggaUtFranKonto()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
