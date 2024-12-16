using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Contact;
using FoodieHub.MVC.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _httpClient;
        public ContactController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(ContactDTO contact)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync("Contacts/AddContact", contact);
            var apiResponse = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
            if (apiResponse!=null && apiResponse.Success)
            {
                NotificationHelper.SetSuccessNotification(this,apiResponse.Message);
                var refererUrl = Request.Headers["Referer"].ToString();
                return Redirect(refererUrl ?? Url.Action("Index", "Products"));
            }
            else
            {
                NotificationHelper.SetErrorNotification(this, apiResponse?.Message);
                return View(contact);
            }
        }
    }
}
