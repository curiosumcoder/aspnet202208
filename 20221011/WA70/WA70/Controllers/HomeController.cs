using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WA70.Models;

namespace WA70.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SessionSettings _ss;

        public HomeController(ILogger<HomeController> logger, SessionSettings ss)
        {
            _logger = logger;
            _ss = ss;
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

            return View();
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