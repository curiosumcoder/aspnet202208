using Northwind.Store.Model;

namespace WA4.ViewModels
{
    public class HomeIndexViewModel
    {
        public string Filter { get; set; } = "";
        public List<CategoryGroupViewModel> Groups { get; set; } = new List<CategoryGroupViewModel>();

        public class CategoryGroupViewModel
        {
            public string CategoryName { get; set; } = "";
            public List<Product> Products { get; set; } = new List<Product>();
        }
    }
}
