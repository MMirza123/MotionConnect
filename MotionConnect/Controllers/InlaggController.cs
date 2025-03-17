using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class InlaggController : Controller 
{
    private readonly UserManager<ApplicationUser> _userManager;

    public InlaggController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult SkapaEttInlagg()
    {
        return View();
    }
}