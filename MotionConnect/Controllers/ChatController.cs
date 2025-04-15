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
        // Skickar med användarens namn och profilbild till vyn
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        // Om inget id skickats med omdirigeras man tillbaka
        if (string.IsNullOrEmpty(anvandarId))
        {
            Console.WriteLine("AnvandarId är null eller tom!");
            return RedirectToAction("ValjChat");
        }

        var inloggadAnvandare = _userManager.GetUserId(User);

        // Hitta eventuell existerande privat chatt mellan inloggad användare och mottagare
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

        // Debug: skriv ut hur många meddelanden som hittades
        Console.WriteLine($"Meddelanden i chatt: {chatt?.Meddelanden?.Count ?? 0}");

        // Skicka med chattens meddelanden och mottagarens id till vyn
        ViewBag.Meddelanden = chatt?.Meddelanden.ToList();
        ViewBag.MottagareId = anvandarId;

        return View();
    }

    // Skickar ett nytt meddelande till en specifik mottagare
    [HttpPost]
    public async Task<IActionResult> SkickaText(string text, string mottagarId)
    {
        var anvandareId = _userManager.GetUserId(User);

        // Försök hitta en existerande chatt mellan dessa två användare
        var chat = await _context.Chattar
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Mottagare)
            .Where(c => !c.ArGruppChat)
            .Where(c => c.Meddelanden.Any(m =>
                (m.AvsandareId == anvandareId && m.Mottagare.Any(mm => mm.MottagareId == mottagarId)) ||
                (m.AvsandareId == mottagarId && m.Mottagare.Any(mm => mm.MottagareId == anvandareId))
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
            AvsandareId = anvandareId,
            ChatId = chat.ChatId,
            SkapadesTid = DateTime.UtcNow,
            Mottagare = new List<MeddelandeMottagare>
            {
                new MeddelandeMottagare { MottagareId = mottagarId }
            }
        };

        _context.Meddelanden.Add(meddelande);
        await _context.SaveChangesAsync();

        // Skicka tillbaka användaren till chattvyn efter att meddelandet skickats
        return RedirectToAction("StartaChat", new { anvandarId = mottagarId });
    }
}
