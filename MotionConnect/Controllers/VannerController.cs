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

    public VannerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> laggTillVan(string id)
    {
        var userId = _userManager.GetUserId(User);
        var anvandare = await _userManager.FindByIdAsync(userId);

        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var vanner = new Van
        {
            AnvandarId = anvandare.Id,
            VanId = id
        };

        _context.Vanner.Add(vanner);
        await _context.SaveChangesAsync();

        return RedirectToAction("KontoInfoAnvandare", "Konto");
    }
}