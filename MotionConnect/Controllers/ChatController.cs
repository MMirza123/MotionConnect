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
        var anvandare = await _context.Users
        .Select(u => new {u.Id, u.ForNamn, u.EfterNamn})
        .ToListAsync();

        ViewBag.Anvandare = anvandare;

        return View(anvandare);
    }

    [HttpGet]
    public async Task<IActionResult> StartaChat(string anvandarId)
    {
        var inloggadAnvandare = _userManager.GetUserId(User);

        var meddelande = await _context.MeddelandeMottagre
        .Where(mm => mm.MottagareId == anvandarId || mm.MottagareId == inloggadAnvandare)
        .GroupBy(mm => mm.Meddelande.ChatId)
        .Where(g => g.Select(x => x.MottagareId).Distinct().Count == 2)
        .Select(g => g.First().Meddelande.Chat)
        .FirstOrDefaultAsync(c => !c.ArGroupChat);

        ViewBag.Meddelande = meddelande;

        return View(meddelande);
    }

}