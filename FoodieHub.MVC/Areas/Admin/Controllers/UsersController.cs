using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }
        public async Task<IActionResult> Index(string? role,string? email,int pageSize=10,int currentPage=1)
        {
            string baseUrl = "users";
            var queryParams = new Dictionary<string, string>();
            queryParams["pageSize"] = pageSize.ToString();
            queryParams["currentPage"] = currentPage.ToString();
            if (!string.IsNullOrEmpty(email))
            {
                queryParams["email"] = email;
                ViewBag.Email = email;
                queryParams["currentPage"] = 1.ToString();
            }
            if (!string.IsNullOrEmpty(role))
            {
                queryParams["role"] = role;
                ViewBag.Role = role;
                queryParams["currentPage"] = 1.ToString();
            }        
            var urlWithQuery = QueryHelpers.AddQueryString(baseUrl, queryParams);

            var httpResponse = await _httpClient.GetAsync(urlWithQuery);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<PaginatedResult<JsonElement>>>();

            if (data != null)
            {
                if (data.Success)
                {
                    return View(data.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUser user)
        {
            if (!ModelState.IsValid)
            {
                return View(user); // Return if the model is invalid
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(user.Fullname), "Fullname");
                content.Add(new StringContent(user.Email), "Email");

                if (!string.IsNullOrEmpty(user.Bio))
                {
                    content.Add(new StringContent(user.Bio), "Bio");
                }

                content.Add(new StringContent(user.IsActive.ToString()), "IsActive");
                content.Add(new StringContent(user.Role), "Role");
                content.Add(new StringContent(user.Password), "Password");

                if (user.File != null && user.File.Length > 0)
                {
                    var fileContent = new StreamContent(user.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(user.File.ContentType);
                    content.Add(fileContent, "File", user.File.FileName);
                }

                // Send the request to the API
                var httpResponse = await _httpClient.PostAsync("users", content);

                // Check if the request was successful
                if (httpResponse.IsSuccessStatusCode)
                {
                    var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                    if (data != null)
                    {
                        if (data.Success)
                        {
                            TempData["SuccessMessage"] = data.Message;
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = data.Message;
                            return View(user);
                        }
                    }
                }

                TempData["ErrorMessage"] = "Server Error. Please try again.";
                return View(user); // Return user to view on failure
            }
        }
        public IActionResult Edit()
        {
            return View();
        }
        public async Task<IActionResult> Detail(string id)
        {
            var response = await _httpClient.GetAsync($"users/{id}");
            var data = await response.Content.ReadFromJsonAsync<APIResponse<UserDTO>>();
            if (data != null && data.Success)
            {
                return View(data.Data);
            }
               
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Disable(string id)
        {
            var response = await _httpClient.PatchAsync($"users/disable/{id}", null);
            if (response != null)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                if (data!=null&& data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to disable user. Please try again";
                }
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Restore(string id)
        {
            var response = await _httpClient.PatchAsync($"users/restore/{id}", null);
            if (response != null)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                if (data != null && data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to restore user. Please try again";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
