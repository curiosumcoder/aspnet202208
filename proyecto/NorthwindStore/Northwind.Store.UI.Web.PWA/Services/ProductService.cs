using Northwind.Store.UI.Web.PWA;
using System.Net.Http.Json;
using System.Text.Json;

namespace Northwind.Store.UI.Web.PWA.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Model.Product>> Search(string filter)
        {
            return await _httpClient.GetFromJsonAsync<List<Model.Product>>($"Search/{filter}");
        }

        public async Task<Model.Product> Create(Model.Product p)
        {
            Model.Product result;

            using (var response = await _httpClient.PostAsJsonAsync<Model.Product>("", p))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Model.Product>(apiResponse);
            }

            return result;
        }

        public async Task Update(Model.Product p)
        {
            using (var response = await _httpClient.PutAsJsonAsync<Model.Product>($"{p.ProductId}", p))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task Delete(int id)
        {
            await _httpClient.DeleteAsync($"{id}");
        }
    }
}
