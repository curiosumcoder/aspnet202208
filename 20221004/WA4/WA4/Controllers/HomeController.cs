using Microsoft.AspNetCore.Mvc;
using Northwind.Store.Data;
using System.Diagnostics;
using WA4.Models;
using static WA4.ViewModels.HomeIndexViewModel;
using Microsoft.EntityFrameworkCore;
using WA4.ViewModels;
using WA4.Extensions;

namespace WA4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NWContext _db;

        public HomeController(ILogger<HomeController> logger, NWContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index(HomeIndexViewModel vm)
        {
            var q1 = from p in _db.Products.Include(p => p.Category).ToList()
                     where p.ProductName.ToLower().Contains(vm.Filter.ToLower())
                     group p by p.Category?.CategoryName ?? "Sin Categoría" into CategoryProducts
                     select new CategoryGroupViewModel()
                     {
                         CategoryName = CategoryProducts.Key,
                         Products = CategoryProducts.ToList()
                     };

            vm.Groups = q1.ToList();

            return View(vm);
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

        public IActionResult IndexPartial(int? id)
        {
            var isAjax = Request.IsAjaxRequest();

            if (id != null)
            {
                return PartialView("ProductPartial", _db.Products.Where(p => p.ProductId == id).SingleOrDefault());
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult IndexViewComponent(int? id)
        {
            var isAjax = Request.IsAjaxRequest();

            if (id != null)
            {
                return ViewComponent("Product", new { id });
            }
            else
            {
                return NotFound();
            }
        }
    }
}