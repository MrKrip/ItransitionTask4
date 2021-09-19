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
        ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _context = db;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
