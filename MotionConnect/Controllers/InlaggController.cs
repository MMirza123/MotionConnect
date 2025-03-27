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
        var userId = _userManager.GetUserId(User);

        var inlagg = await _context.Inlagg
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
            .ThenInclude(isport => isport.Sport) 
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        var antalGillningarPerInlagg = await _context.Gillningar
        .GroupBy(g => g.InlaggId)
        .ToDictionaryAsync(g => g.Key, g => g.Count());

        var harGillad = await _context.Gillningar
        .Where(g => g.AnvandarId == userId)
        .Select(g => g.InlaggId)
        .ToListAsync();

        ViewBag.HarGillad = harGillad;
        ViewBag.AntalGillningar = antalGillningarPerInlagg;

        return View(inlagg ?? new List<Inlagg>());
    }

    [HttpGet]
    public async Task<IActionResult> VisaInlaggAnvandare()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("LoggaInPaKonto", "Konto");
        }

        var userId = _userManager.GetUserId(User);

        var inlagg = await _context.Inlagg
            .Where(i => i.AnvandarId == userId)
            .Include(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
            .ThenInclude(isport => isport.Sport) 
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();
        
        return View(inlagg);
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

    [HttpPost]
    public async Task<IActionResult> taBortInlagg(int id)
    {
        var inlaggId = await _context.Inlagg.FindAsync(id);

        if(inlaggId == null)
        {
            return NotFound();
        }

        _context.Inlagg.Remove(inlaggId);
        await _context.SaveChangesAsync();

        return RedirectToAction("VisaInlaggAnvandare", "Inlagg");
    }

    [HttpPost]
    public async Task<IActionResult> GillaInlagg(int id)
    {
        var inlagg = await _context.Inlagg.FindAsync(id);
        var anvandareId = _userManager.GetUserId(User);

        if (inlagg == null)
        {
            return NotFound();
        }

        var gillning = new Gillning
        {
            InlaggId = inlagg.InlaggId,
            AnvandarId = anvandareId
        };

        _context.Gillningar.Add(gillning);
        await _context.SaveChangesAsync();

        return RedirectToAction("VisaInlagg", "Inlagg");
    }

}