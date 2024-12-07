using FoodieHub.MVC.Models.Contact;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Controllers
{
    public class Contact : Controller
    {
        private readonly HttpClient _httpClient;
        public Contact(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(ContactDTO contact)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("Contact/AddContact", contact);
            var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
            if (apiResponse.Success)
            {
                TempData["SuccessMessage"] = apiResponse.Message;
                var refererUrl = Request.Headers["Referer"].ToString();
                return Redirect(refererUrl ?? Url.Action("Index", "Products"));
            }
            else
            {
                TempData["ErrorMessage"] = apiResponse.Message;
                return View(contact);
            }
        }
    }
}
