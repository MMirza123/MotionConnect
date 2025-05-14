using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

// Controller som hanterar all logik kring chattfunktionaliteten
public class ChatController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    // Konstruktor som injicerar UserManager, SignInManager och databaskontexten
    public ChatController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // Visar en lista över alla användare man kan starta en chatt med
    [HttpGet]
    public async Task<IActionResult> ValjChat()
    {
        // Om en användare är inloggad – skicka med namn och profilbild till vyn via ViewBag
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        // Hämta alla användare från databasen med endast deras id och namn
        var anvandare = await _context.Users
            .Select(u => new { u.Id, u.ForNamn, u.EfterNamn })
            .ToListAsync();

        ViewBag.Anvandare = anvandare;

        return View(anvandare);
    }

    // Startar en privat chatt mellan den inloggade användaren och en annan användare
    [HttpGet]
    public async Task<IActionResult> StartaChat(string anvandarId)
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
            ViewBag.InloggadId = user.Id;
        }

        if (string.IsNullOrEmpty(anvandarId))
        {
            Console.WriteLine("AnvandarId är null eller tom!");
            return RedirectToAction("ValjChat");
        }

        var inloggadAnvandare = _userManager.GetUserId(User);

        // 🛠 LÄGG IN DEN HÄR!
        var mottagare = await _context.Users
            .Where(u => u.Id == anvandarId)
            .Select(u => new { u.ForNamn, u.EfterNamn })
            .FirstOrDefaultAsync();

        if (mottagare != null)
        {
            ViewBag.MottagareNamn = $"{mottagare.ForNamn} {mottagare.EfterNamn}";
        }

        var chatt = await _context.Chattar
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Avsandare)
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Mottagare)
            .Where(c => !c.ArGruppChat)
            .Where(c => c.Meddelanden.Any(m =>
                (m.AvsandareId == inloggadAnvandare && m.Mottagare.Any(mm => mm.MottagareId == anvandarId)) ||
                (m.AvsandareId == anvandarId && m.Mottagare.Any(mm => mm.MottagareId == inloggadAnvandare))
            ))
            .FirstOrDefaultAsync();

        Console.WriteLine($"Meddelanden i chatt: {chatt?.Meddelanden?.Count ?? 0}");

        ViewBag.Meddelanden = chatt?.Meddelanden.ToList();
        ViewBag.MottagareId = anvandarId;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SkapaGruppChat()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        var anvandare = await _context.Users
            .Select(u => new { u.Id, u.ForNamn, u.EfterNamn })
            .ToListAsync();

        ViewBag.Anvandare = anvandare;

        return View(anvandare);
    }

    [HttpGet]
    public async Task<IActionResult> VisaGruppChat(int gruppId)
    {

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        var anvandarId = _userManager.GetUserId(User);
        ViewBag.InloggadId = _userManager.GetUserId(User);

        var grupp = await _context.Grupper
            .Include(g => g.GruppMedlemmar)
                .ThenInclude(gm => gm.Anvandare)
            .FirstOrDefaultAsync(g => g.GruppId == gruppId);

        var chat = await _context.Chattar
            .Where(c => c.ArGruppChat && c.GroupName == gruppId.ToString()) // <-- ändrat här
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Avsandare)
            .FirstOrDefaultAsync();

        ViewBag.Meddelanden = chat?.Meddelanden?.OrderBy(m => m.SkapadesTid).ToList() ?? new List<Meddelande>();

        return View(grupp);
    }


    [HttpPost]
    public async Task<IActionResult> StartaGruppChat(string gruppNamn, List<string> valdaAnvandareIds)
    {
        var inloggadId = _userManager.GetUserId(User);

        // Skapa chatt och grupp
        var nyChatt = new Chat { ArGruppChat = true, GroupName = gruppNamn };
        _context.Chattar.Add(nyChatt);
        await _context.SaveChangesAsync();

        var nyGrupp = new Grupp { GruppNamn = gruppNamn };
        _context.Grupper.Add(nyGrupp);
        await _context.SaveChangesAsync();

        // Lägg till alla användare + den inloggade i gruppen
        var allaMedlemmar = valdaAnvandareIds.Append(inloggadId).Distinct();

        foreach (var userId in allaMedlemmar)
        {
            _context.GruppMedlemmar.Add(new GruppMedlem
            {
                GruppId = nyGrupp.GruppId,
                AnvandarId = userId
            });
        }

        await _context.SaveChangesAsync();

        /*return RedirectToAction("VisaGruppChat", new { gruppId = nyGrupp.GruppId });*/
        return RedirectToAction("Index", "Home");
    }

    // Skickar ett nytt meddelande till en specifik mottagare
    [HttpPost]
    public async Task<IActionResult> SkickaText(string text, string mottagarId)
    {
        var userId = _userManager.GetUserId(User);

        // Försök hitta en existerande chatt mellan dessa två användare
        var chat = await _context.Chattar
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Mottagare)
            .Where(c => !c.ArGruppChat)
            .Where(c => c.Meddelanden.Any(m =>
                (m.AvsandareId == userId && m.Mottagare.Any(mm => mm.MottagareId == mottagarId)) ||
                (m.AvsandareId == mottagarId && m.Mottagare.Any(mm => mm.MottagareId == userId))
            ))
            .FirstOrDefaultAsync();

        // Om det inte finns någon tidigare chatt, skapa en ny
        if (chat == null)
        {
            chat = new Chat { ArGruppChat = false, SkapadTid = DateTime.UtcNow };
            _context.Chattar.Add(chat);
            await _context.SaveChangesAsync();
        }

        // Skapa ett nytt meddelande och koppla till chatten och mottagaren
        var meddelande = new Meddelande
        {
            Text = text,
            AvsandareId = userId,
            ChatId = chat.ChatId,
            SkapadesTid = DateTime.UtcNow,
            Mottagare = new List<MeddelandeMottagare>
            {
                new MeddelandeMottagare { MottagareId = mottagarId }
            }
        };

        var notis = new Notis
        {
            Meddelande = text,
            Typ = NotisTyp.Meddelande,
            ArLast = false,
            AnvandarId = mottagarId,
            AvsandareId = userId,
            SkapadesTid = DateTime.UtcNow
        };

        _context.Meddelanden.Add(meddelande);
        _context.Notiser.Add(notis);
        await _context.SaveChangesAsync();

        // Skicka tillbaka användaren till chattvyn efter att meddelandet skickats
        return RedirectToAction("StartaChat", new { anvandarId = mottagarId });
    }

    [HttpPost]
    public async Task<IActionResult> SkickaTillGrupp(string text, int gruppId)
    {
        var anvandareId = _userManager.GetUserId(User);

        var medlem = await _context.GruppMedlemmar
            .FirstOrDefaultAsync(m => m.AnvandarId == anvandareId && m.GruppId == gruppId);

        if (medlem == null)
            return Unauthorized();

        var chat = await _context.Chattar
            .FirstOrDefaultAsync(m => m.ArGruppChat && m.GroupName == gruppId.ToString());

        if (chat == null)
        {
            chat = new Chat
            {
                ArGruppChat = true,
                GroupName = gruppId.ToString(),
                SkapadTid = DateTime.UtcNow
            };
            _context.Chattar.Add(chat);
            await _context.SaveChangesAsync();
        }

        var meddelande = new Meddelande
        {
            Text = text,
            AvsandareId = anvandareId,
            ChatId = chat.ChatId,
            SkapadesTid = DateTime.UtcNow,
            Mottagare = new List<MeddelandeMottagare>()
        };

        var medlemmar = await _context.GruppMedlemmar
            .Where(m => m.GruppId == gruppId && m.AnvandarId != anvandareId)
            .ToListAsync();

        foreach (var m in medlemmar)
        {
            meddelande.Mottagare.Add(new MeddelandeMottagare
            {
                MottagareId = m.AnvandarId
            });
        }

        _context.Meddelanden.Add(meddelande);
        await _context.SaveChangesAsync();

        return RedirectToAction("VisaGruppchat", new { gruppid = gruppId });
    }
}
