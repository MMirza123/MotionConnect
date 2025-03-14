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
        Console.WriteLine("🔵 GET SkapaKonto anropad!");
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SkapaKonto(RegisterViewModel model)
    {
        Console.WriteLine("🔄 SkapaKonto-metoden anropades!");
        if (!ModelState.IsValid)
        {
            Console.WriteLine("❌ ModelState är ogiltig!");
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    Console.WriteLine($"❌ Fel i {key}: {error.ErrorMessage}");
                }
            }
            return View(model);
        }

        Console.WriteLine($"📧 E-post: {model.Email}");
        Console.WriteLine($"📞 Telefonnummer: {model.Telefonnummer}");
        Console.WriteLine($"📂 Profilbild: {(model.Profilbild != null ? model.Profilbild.FileName : "Ingen bild vald")}");

        var anvandare = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.Telefonnummer,
            ForNamn = model.Fornamn,
            EfterNamn = model.Efternamn,
            FodelsAr = model.Fodelsear,
            ArProfilOppen = model.ArProfilOppen,
            ProfilBildUrl = "/uploads/default.png"
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
                Console.WriteLine($"❌ FEL vid sparande av bild: {ex.Message}");
                ModelState.AddModelError("", "Kunde inte spara profilbilden.");
                ModelState.AddModelError("", "Kunde inte spara profilbilden.");
                return View(model);
            }
        }

        var result = await _userManager.CreateAsync(anvandare, model.Losenord);
        if (!result.Succeeded)
        {
            Console.WriteLine("❌ UserManager.CreateAsync() FAILED!");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"❌ Identity Error: {error.Description}");
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        Console.WriteLine($"✅ Användaren {anvandare.Email} har skapats!");

        // 🛠 Kolla om användaren loggas in
        await _signInManager.SignInAsync(anvandare, isPersistent: false);
        Console.WriteLine("🔑 Användaren loggades in!");
        return RedirectToAction("Index", "Home");
    }


}
