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
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        var sporter = await _context.Sporter.ToListAsync();
        ViewBag.Sporter = sporter; // Skickar listan med sporter till vyn

        return View();
    }


    [HttpGet]
    public async Task<IActionResult> VisaInlagg()
    {
        var inlagg = await _context.Inlagg
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
            .ThenInclude(isport => isport.Sport) 
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        return View(inlagg ?? new List<Inlagg>());
    }

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

        // Hantera bilduppladdning
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

        // Skapa inlägget
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

        // Koppla inlägget till valda sporter
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

        return RedirectToAction("VisaInlagg");
    }

}