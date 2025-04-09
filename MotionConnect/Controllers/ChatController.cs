using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;


public class ChatController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public ChatController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ValjChat()
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
    public async Task<IActionResult> StartaChat(string anvandarId)
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.AnvandareNamn = $"{user.ForNamn} {user.EfterNamn}";
            ViewBag.ProfilBild = user.ProfilBildUrl;
        }

        if (string.IsNullOrEmpty(anvandarId))
        {
            Console.WriteLine("❌ anvandarId är null eller tom!");
            return RedirectToAction("ValjChat");
        }

        var inloggadAnvandare = _userManager.GetUserId(User);

        // Hitta chatten där båda är med (som avsändare/mottagare)
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

        Console.WriteLine($"📥 Meddelanden i chatt: {chatt?.Meddelanden?.Count ?? 0}");

        ViewBag.Meddelanden = chatt?.Meddelanden.ToList();

        ViewBag.MottagareId = anvandarId;

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SkickaText(string text, string mottagarId)
    {
        var anvandareId = _userManager.GetUserId(User);

        // Hitta existerande privat chatt mellan de två
        var chat = await _context.Chattar
            .Include(c => c.Meddelanden)
                .ThenInclude(m => m.Mottagare)
            .Where(c => !c.ArGruppChat)
            .Where(c => c.Meddelanden.Any(m =>
                (m.AvsandareId == anvandareId && m.Mottagare.Any(mm => mm.MottagareId == mottagarId)) ||
                (m.AvsandareId == mottagarId && m.Mottagare.Any(mm => mm.MottagareId == anvandareId))
            ))
            .FirstOrDefaultAsync();

        // Om ingen finns – skapa ny
        if (chat == null)
        {
            chat = new Chat { ArGruppChat = false, SkapadTid = DateTime.UtcNow };
            _context.Chattar.Add(chat);
            await _context.SaveChangesAsync();
        }

        // Skapa och koppla meddelande
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

        return RedirectToAction("StartaChat", new { anvandarId = mottagarId });
    }


}