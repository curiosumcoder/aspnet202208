namespace WA50.Models
{
    public class HomeIndexModel
    {
        public string Filter { get; set; } = "";
        public List<Northwind.Store.Model.Product> Items { get; set; }
    }
}
