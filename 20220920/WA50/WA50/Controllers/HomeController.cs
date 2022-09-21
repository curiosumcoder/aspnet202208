using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WA50.Models;

namespace WA50.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(HomeIndexModel m)
        {
            if (m.Filter != "")
            {
                using (var db = new Northwind.Store.Data.NWContext())
                {
                    m.Items = db.Products.Where(p => p.ProductName.Contains(m.Filter)).ToList();
                }
            }

            return View(m);
        }

        //[HttpPost]
        //[HttpGet]
        //public IActionResult Index([FromRoute] string filter="")
        //public IActionResult Index(string filter = "")
        public IActionResult Index1(HomeIndexModel m)
        {
            //List<Northwind.Store.Model.Product> result = new();

            //if (filter != "")
            if (m.Filter != "")
            {
                using (var db = new Northwind.Store.Data.NWContext())
                {
                    //result = db.Items.Where(p => p.ProductName.Contains(filter)).ToList();

                    m.Items = db.Products.Where(p => p.ProductName.Contains(m.Filter)).ToList();
                }
            }

            return View(m);
        }

        public IActionResult Index0(string filter)
        {
            ViewData[nameof(filter)] = filter;
            ViewBag.filter2 = filter;

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