using FoodieHub.MVC.Models.Request;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http;

namespace FoodieHub.MVC.Service.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

      

        public async Task<APIResponse> Login(LoginVM loginVM)
        {
            var client = _httpClientFactory.CreateClient("MyAPI");
            var response = await client.PostAsJsonAsync("auth/admin/login", loginVM);

            var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();
            return apiResponse;
        }

        
    }
}
