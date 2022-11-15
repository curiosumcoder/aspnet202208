using Northwind.Store.Model;
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

        public async Task<List<Product>> Search(string filter)
        {
            return await _httpClient.GetFromJsonAsync<List<Product>>($"Search/{filter}");
        }

        public async Task<Product> Create(Product p)
        {
            Product result;

            using (var response = await _httpClient.PostAsJsonAsync<Product>("", p))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<Product>(apiResponse);
            }

            return result;
        }

        public async Task Update(Product p)
        {
            using (var response = await _httpClient.PutAsJsonAsync<Product>($"{p.ProductId}", p))
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
