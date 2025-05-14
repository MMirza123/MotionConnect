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
    public async Task<IActionResult> KommentarInlagg(int? sportId, int inlaggId)
    {
        //Kollar om det är en användare inloggad
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User); //Hämtar den inloggande användarens uppgifter
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}"; //En säck som skickas till html sidan med "För- och efternamn på den inloggade användaren"
            ViewBag.ProfilBild = user.ProfilBildUrl; //En säck som skickas med profilbild för den inloggade användaren
        }

        //Om ingen är inloggad ska en tom HomeViewModel skickas
        if (!User.Identity.IsAuthenticated)
            return View(new HomeViewModel());

        //Hämmtar hela namnet på den inloggade användaren
        string identifier = User.Identity.Name;
        //Hitta mailen(unika nyckeln) på anävändaren går bara att hitta med hela namnet, annars hitta namnet
        var anvandare = await _userManager.FindByEmailAsync(identifier) ?? await _userManager.FindByNameAsync(identifier);
        //Hämta användarens unika id
        var anvandarId = _userManager.GetUserId(User);

        //Hätar dett inlägg där den inkomande parametern med inlaggsId matchar inläggsIdet som finns i databasen.
        //Det som tas ed är användaren som skapade inlägget och sporterna som tillhör inlägget
        var inlaggQuery = _context.Inlagg
            .Include(i => i.Anvandare)
            .Include(i => i.Kommentarer)
                .ThenInclude(i => i.Anvandare)
            .Include(i => i.InlaggSporter)
                .ThenInclude(isport => isport.Sport)
            .AsQueryable()
            .Where(i => i.InlaggId == inlaggId)
            .AsQueryable();

        if (sportId.HasValue)
        {
            inlaggQuery = inlaggQuery
                .Where(i => i.InlaggSporter.Any(s => s.SportId == sportId));
        }

        var inlagg = await inlaggQuery
            .OrderByDescending(i => i.SkapadesTid)
            .ToListAsync();

        // Sortera kommentarerna i varje inlägg i stigande ordning (äldsta först)
        foreach (var i in inlagg)
        {
            i.Kommentarer = i.Kommentarer.OrderBy(k => k.SkapadTid).ToList();
        }


        ViewBag.ValdSportId = sportId;

        var sporterMedInlagg = await _context.InlaggSporter
            .Include(i => i.Sport)
            .GroupBy(i => i.SportId)
            .Select(i => i.First().Sport)
            .ToListAsync();

        ViewBag.Sporter = sporterMedInlagg;

        // Räkna hur många gillningar varje inlägg har
        var antalGillningarPerInlagg = await _context.Gillningar
            .GroupBy(g => g.InlaggId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        // Hämta lista på inlägg som den inloggade användaren har gillat
        var harGillad = await _context.Gillningar
            .Where(g => g.AnvandarId == anvandarId)
            .Select(g => g.InlaggId)
            .ToListAsync();

        var antalKommentarePerInlagg = await _context.Kommentarer
            .GroupBy(k => k.InlaggId)
            .ToDictionaryAsync(k => k.Key, k => k.Count());

        var model = new HomeViewModel
        {
            Anvandare = anvandare,
            HarGillatInlaggIds = harGillad,
            AntalGillningar = antalGillningarPerInlagg,
            AntalKommentarer = antalKommentarePerInlagg,
            Inlagg = inlagg ?? new List<Inlagg>()
        };

        return View(model);
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
    public async Task<IActionResult> GillaInlagg(int id, string returnUrl)
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

        _context.Gillningar.Add(gillning);

        if (anvandareId != inlagg.AnvandarId)
        {
            // Skapar en notis till den som äger inlägget
            var notis = new Notis
            {
                Meddelande = $"{anvandare.ForNamn} {anvandare.EfterNamn} har gillad dit inlägg",
                Typ = NotisTyp.Gillning,
                ArLast = false,
                AnvandarId = inlagg.AnvandarId,
                AvsandareId = anvandareId,
                SkapadesTid = DateTime.UtcNow,
                InlaggId = id
            };

            _context.Notiser.Add(notis);
        }

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("KommentarInlagg"))
        {
            var inlaggIdQuery = System.Web.HttpUtility.ParseQueryString(new Uri("http://dummy" + returnUrl).Query);
            var inlaggId = int.Parse(inlaggIdQuery["inlaggId"]);
            return RedirectToAction("KommentarInlagg", "Inlagg", new { inlaggId });
        }

        return Redirect(returnUrl ?? "/");
    }

    // Tar bort gillning och tillhörande notis
    [HttpPost]
    public async Task<IActionResult> taBortGillning(int id, string returnUrl)
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

        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(returnUrl) && returnUrl.Contains("KommentarInlagg"))
        {
            var inlaggIdQuery = System.Web.HttpUtility.ParseQueryString(new Uri("http://dummy" + returnUrl).Query);
            var inlaggId = int.Parse(inlaggIdQuery["inlaggId"]);
            return RedirectToAction("KommentarInlagg", "Inlagg", new { inlaggId });
        }

        return Redirect(returnUrl ?? "/");
    }

    [HttpPost]
    public async Task<IActionResult> skickaKommentar(string text, int inlaggId)
    {
        var user = await _userManager.GetUserAsync(User);
        ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
        ViewBag.ProfilBild = user.ProfilBildUrl;

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var kommentar = new Kommentar
        {
            Text = text,
            SkapadTid = DateTime.UtcNow,
            InlaggId = inlaggId,
            AnvandareId = userId
        };

        _context.Kommentarer.Add(kommentar);
        await _context.SaveChangesAsync();

        return RedirectToAction("KommentarInlagg", new { inlaggId });
    }
}
