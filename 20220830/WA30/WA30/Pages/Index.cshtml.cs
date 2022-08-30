using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using Northwind.Store.Model;
using Northwind.Store.Data;

namespace WA30.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<Product> Products { get; set; }

        [BindProperty()]
        public string Filter { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string filter)
        {
            using (var db = new NWContext())
            {
                Products = db.Products.Where(
                    p => p.ProductName.Contains(filter ?? "")).ToList();
            }
        }
    }
}