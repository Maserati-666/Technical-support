using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Technical_support.Models;

namespace Technical_support.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (HttpContext.User.IsInRole("Client"))
            {
                return Redirect("/Client/Index");
            }


            if (HttpContext.User.IsInRole("Admin"))
            {
                return Redirect("/Admin/Index");
            }

            return HttpContext.User.IsInRole("Manager") ? Redirect("/Manager/Index") : View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}