using Northwind.Store.Model;

namespace Northwind.Store.UI.Web.PWA.Client.Services
{
    public interface IProductService
    {
        Task<List<Product>> Search(string filter);
        Task<Product> Create(Product p);
        Task Update(Product p);
        Task Delete(int id);
    }
}
