
namespace Northwind.Store.UI.Web.PWA.Client.Services
{
    public interface IProductService
    {
        Task<List<Model.Product>> Search(string filter);
        Task<Model.Product> Create(Model.Product p);
        Task Update(Model.Product p);
        Task Delete(int id);
    }
}
