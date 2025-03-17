using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class KontoController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public KontoController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult SkapaKonto()
    {
        return View();
    }

    [HttpGet]
    public IActionResult LoggaInPaKontot()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> KontoInfo()
    {
        string identifire = User.Identity.Name;
        var anvendare = await _userManager.FindByEmailAsync(identifire);
        return View(anvendare);
    }


    [HttpPost]
    public async Task<IActionResult> SkapaKonto(RegisterViewModel model)
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
        // 🛠 Kolla om användaren loggas in
        await _signInManager.SignInAsync(anvandare, isPersistent: false);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> LoggaInPaKontot(string email, string losenord)
    {
        var anvandare = await _userManager.FindByEmailAsync(email);

        if(anvandare == null)
        {
            Console.WriteLine($"Anändaren med epost{email} finns inte");
            ModelState.AddModelError("", "Användaren finns inte");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(email, losenord, isPersistent: false, lockoutOnFailure: false);

        if(result.Succeeded)
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
