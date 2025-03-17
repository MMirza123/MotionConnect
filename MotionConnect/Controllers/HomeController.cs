using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MotionConnect.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace MotionConnect.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManger;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManger)
    {
        _logger = logger;
        _userManger = userManger;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
                return View(null);

            string identifier = User.Identity.Name;
            var anvandare = await _userManger.FindByEmailAsync(identifier) ?? await _userManger.FindByNameAsync(identifier);
                            
            return View(anvandare);
        }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
