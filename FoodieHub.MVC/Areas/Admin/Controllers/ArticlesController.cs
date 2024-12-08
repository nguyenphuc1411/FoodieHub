using Azure;
using FoodieHub.API.Models.Response;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Category;
using FoodieHub.MVC.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class ArticlesController : Controller
    {
        private readonly HttpClient _httpClient;

        public ArticlesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IActionResult> Index(string? search,int? categoryID,int pageSize=10,int currentPage=1)
        {
            string baseUrl = "articles/foradmin";
            var queryParams = new Dictionary<string, string>();
            queryParams["pageSize"] = pageSize.ToString();
            queryParams["currentPage"] = currentPage.ToString();
            if (!string.IsNullOrEmpty(search))
            {
                queryParams["search"] = search;
                ViewBag.Search = search;
                queryParams["currentPage"] = 1.ToString();
            }

            if (categoryID.HasValue && categoryID.Value!=0)
            {
                queryParams["categoryID"] = categoryID.Value.ToString();
                queryParams["currentPage"] = 1.ToString();
            }
            var urlWithQuery = QueryHelpers.AddQueryString(baseUrl, queryParams);

            var httpResponse = await _httpClient.GetAsync(urlWithQuery);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<PaginatedModel<JsonElement>>>();

            if (data != null)
            {
                if (data.Success)
                {
                    var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("ArticleCategory/getallarticlecategory");
                    if (response != null)
                    {
                        ViewBag.Category = response.ToList();
                    }
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
                return RedirectToAction("Index","Home");
            }
        }
    

        public async Task<IActionResult> Create()
        {
            var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("ArticleCategory/getallarticlecategory");
            if (response != null)
            {
                ViewBag.Category = response.Where(x=>x.IsDeleted==false).ToList();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateArticle article)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các thông tin khác của Article
                    content.Add(new StringContent(article.Title), "Title");
                    content.Add(new StringContent(article.Description), "Description");
                    content.Add(new StringContent(article.CategoryID.ToString()), "CategoryID");

                    var fileContent = new StreamContent(article.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(article.File.ContentType);
                    content.Add(fileContent, "File", article.File.FileName);

                    var httpResponse = await _httpClient.PostAsync("articles", content);

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
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Server Error. Please try again.";
                    }
                }
            }
            var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("ArticleCategory/getallarticlecategory");
            if (response != null)
            {
                ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
            }
            return View(article);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var httpResponse = await _httpClient.GetAsync($"articles/{id}");
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<JsonObject>>();     
            if (data != null)
            {
                if (data.Success)
                {                 
                    return View(data.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Server Error. Please try again.";
                return RedirectToAction("Index");
            }
            
        }
        public async Task<IActionResult> Edit(int id)
        {
            var httpResponse = await _httpClient.GetAsync($"articles/foradmin/{id}");
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<JsonElement>>();

            if (data != null && data.Success)
            {                       
                var article = new EditArticle
                {
                    ArticleID = data.Data.GetProperty("articleID").GetInt32(),
                    Title = data.Data.GetProperty("title").GetString(),
                    Description = data.Data.GetProperty("description").GetString(), 
                    CategoryID = data.Data.GetProperty("categoryID").GetInt32()
                };
                HttpContext.Session.SetString("ImageEditArticle", data.Data.GetProperty("mainImage").ToString());
                ViewBag.CurrentImage = data.Data.GetProperty("mainImage").ToString();
                var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("ArticleCategory/getallarticlecategory");
                if (response != null)
                {
                    ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
                }
                return View(article);
            }
            else if (data != null)
            {
                TempData["ErrorMessage"] = data.Message;
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditArticle edit)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(edit.Title), "Title");
                    content.Add(new StringContent(edit.Description), "Description");
                    content.Add(new StringContent(edit.CategoryID.ToString()), "CategoryID");
                    if (edit.File != null && edit.File.Length > 0)
                    {
                        var fileContent = new StreamContent(edit.File.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(edit.File.ContentType);
                        content.Add(fileContent, "File", edit.File.FileName);
                    }

                    // Gửi yêu cầu đến API
                    var httpResponse = await _httpClient.PutAsync($"articles/{edit.ArticleID}", content);
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
                        }
                    }
                    else
                    {                      
                        TempData["ErrorMessage"] = "Server Error. Please try again.";                   
                    }

                }
            }
            var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("ArticleCategory/getallarticlecategory");
            if (response != null)
            {
                ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
            }
            var currentImage =HttpContext.Session.GetString("ImageEditArticle");
            ViewBag.CurrentImage = currentImage;
            return View(edit);
        }
          

        public async Task<IActionResult> Delete(int id)
        {
            var httpResponse = await _httpClient.GetAsync($"articles/{id}");
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<JsonObject>>();
            if (data != null && data.Success)
            {
                return View(data.Data);
            }
            else
            {
                if (data != null)
                {
                    TempData["ErrorMessage"] = data.Message;
                    return RedirectToAction("Index");
                }
            }
            TempData["ErrorMessage"] = "Server Error. Please try again.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"articles/soft/{id}");
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();

            if (data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deleted(int pageSize = 10, int currentPage = 1)
        {
            string baseUrl = "articles/foradmin";
            var queryParams = new Dictionary<string, string>();
            queryParams["pageSize"] = pageSize.ToString();
            queryParams["currentPage"] = currentPage.ToString();          
            queryParams["isDeleted"] = true.ToString();
            var urlWithQuery = QueryHelpers.AddQueryString(baseUrl, queryParams);

            var response = await _httpClient.GetFromJsonAsync<APIResponse<PaginatedModel<JsonElement>>>(urlWithQuery);

            if (response != null)
            {
                return View(response.Data);
            }

            return RedirectToAction();
        }

        public async Task<IActionResult> Restore(int id)
        {
            var httpResponse = await _httpClient.PatchAsync($"articles/restore/{id}",null);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();

            if (data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                }           
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }
            return RedirectToAction("Deleted");
        }
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"articles/hard/{id}");
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
            if(data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                }             
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            }           
            return RedirectToAction("Deleted");
        }
    }
}
