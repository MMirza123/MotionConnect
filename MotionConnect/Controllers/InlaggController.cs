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

    [HttpGet]
    public async Task<IActionResult> SkapaEttInlagg()
    {
        var vemInloggad = User.Identity.Name;
        var anvandare = await _userManager.FindByEmailAsync(vemInloggad);
        return View(anvandare);
    }

    [HttpGet]
    public async Task<IActionResult> VisaInlagg()
    {
        var inlagg = await _context.Inlagg
            .Include(i => i.Anvandare)
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

       return View(inlagg ?? new List<Inlagg>());
    }

    [HttpPost]
public async Task<IActionResult> SkapaEttInlagg(string text, IFormFile bild)
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

    string bildUrl = null;
    
    if (bild != null && bild.Length > 0)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"❌ FEL vid sparande av bild: {ex.Message}");
            ModelState.AddModelError("", "Kunde inte spara bilden.");
            return View();
        }
    }

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
    Console.WriteLine("✅ Inlägg sparat i databasen!");

    return RedirectToAction("VisaInlagg");
}
}