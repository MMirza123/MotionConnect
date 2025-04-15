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

        var notis = new Notis
        {
            Meddelande = $"{anvandare.ForNamn} {anvandare.EfterNamn} har blivit din vän",
            Typ = NotisTyp.Annat,
            ArLast = false,
            AnvandarId = id,
            SkapadesTid = DateTime.UtcNow
        };

        _context.Vanner.Add(vanner);
        _context.Notiser.Add(notis);
        await _context.SaveChangesAsync();

        return RedirectToAction("KontoInfo", "Konto", new { id = id });
    }

    [HttpPost]
    public async Task<IActionResult> taBortVan(string id)
    {
        var userId = _userManager.GetUserId(User);
        
        var vanrelation = await _context.Vanner
            .FirstOrDefaultAsync(v => 
            (v.AnvandarId == userId && v.VanId == id) || 
            (v.AnvandarId == id && v.VanId == userId));

        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (vanrelation != null)
        {
            _context.Vanner.Remove(vanrelation);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("KontoInfo", "Konto", new { id = id });
        
    }

}