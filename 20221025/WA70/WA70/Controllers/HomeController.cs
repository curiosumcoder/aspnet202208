using Microsoft.AspNetCore.Mvc;
using Northwind.Store.Data;
using System.Diagnostics;
using WA70.Models;
using Microsoft.EntityFrameworkCore;

namespace WA70.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SessionSettings _ss;
        private readonly NWContext _db;

        public HomeController(ILogger<HomeController> logger, SessionSettings ss, NWContext db)
        {
            _logger = logger;
            _ss = ss;
            _db = db;
        }

        public IActionResult Index()
        {
            var starTime = HttpContext.Items["StartTime"];

            #region Session Ej1
            //var welcome = "";
            //if (HttpContext.Session.GetString("welcome") == null)
            //{
            //    welcome = $"Welcome {DateTime.Now.ToString()}";
            //    HttpContext.Session.SetString("welcome", welcome);
            //}
            //else
            //{
            //    welcome = HttpContext.Session.GetString("welcome");
            //}
            //ViewBag.welcome = welcome;
            #endregion

            #region Session Ej2
            _ss.Welcome = $"Welcome {DateTime.Now.ToString()}";
            ViewBag.welcome = _ss.Welcome;
            #endregion

            return View(_db.Products.Include(p => p.Category).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}