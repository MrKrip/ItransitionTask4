using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task4.Data;
using Task4.Models;

namespace Task4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = db;
            _userManager=userManager;
        }

        public IActionResult Index()
        {
            var UserInfo = from u in _context.Users
                           join ul in _context.UserLogins on u.Id equals ul.UserId
                           into um from userm in um.DefaultIfEmpty()
                           select (userm.ProviderDisplayName != null
                            ? new ChartModel {y=1, name = userm.ProviderDisplayName }
                            : new ChartModel {y=1, name = "Our System" });
            var temp = UserInfo.ToList().GroupBy(x => x.name).Select(c => new ChartModel { y = c.Count(), name = c.Key });
            return View(temp);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UsersManager()
        {
            var Users = _context.Users.Select(x=>new UserModel { Id=x.Id,UserName=x.UserName,Selected=false });
            return View(Users);
        }

        [HttpPost]
        public async Task<IActionResult> UsersManager(string[] userId, string type)
        {
            if (userId != null)
            {
                foreach (var id in userId)
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (type == "Delete")
                    {
                        if (user != null)
                        {
                            IdentityResult result = await _userManager.DeleteAsync(user);
                            if (result.Succeeded)
                                return RedirectToAction("Index");
                            else
                                Error();
                        }
                    }
                }               
            }
            return RedirectToAction("UsersManager");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
