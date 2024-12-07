using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Category;
using FoodieHub.MVC.Models.Recipe;
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
    public class RecipesController : Controller
    {
        private readonly HttpClient _httpClient;
        public RecipesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }
        public async Task<IActionResult> Index(string? search, int? categoryID, bool? isActive, int pageSize = 10, int currentPage = 1)
        {
            string baseUrl = "recipes/foradmin";
            var queryParams = new Dictionary<string, string>();
            queryParams["pageSize"] = pageSize.ToString();
            queryParams["currentPage"] = currentPage.ToString();
            if (!string.IsNullOrEmpty(search))
            {
                queryParams["search"] = search;
                ViewBag.Search = search;
                queryParams["currentPage"] = 1.ToString();
            }
            if (isActive.HasValue)
            {
                queryParams["isActive"] = isActive.Value.ToString();
                ViewBag.isActive = isActive.Value;
            }
            if (categoryID.HasValue && categoryID.Value != 0)
            {
                queryParams["categoryID"] = categoryID.Value.ToString();
                queryParams["currentPage"] = 1.ToString();
            }

            var urlWithQuery = QueryHelpers.AddQueryString(baseUrl, queryParams);

            var httpResponse = await _httpClient.GetAsync(urlWithQuery);
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<PaginatedResult<JsonElement>>>();

            if (data != null)
            {
                if (data.Success)
                {
                    var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("RecipeCategory/getallrecipecategory");
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
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> Create()
        {
            var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("RecipeCategory/getallrecipecategory");
            if (response != null)
            {
                ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
            }
            return View();
        }
    /*    [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipe)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    TimeOnly preptime = new TimeOnly(recipe.PrepHours, recipe.PrepMinutes);
                    TimeOnly cooktime = new TimeOnly(recipe.CookHours, recipe.CookMinutes);
                    // Thêm các thông tin khác của Article
                    content.Add(new StringContent(recipe.Title), "Title");
                    content.Add(new StringContent(preptime.ToString()), "PrepTime");
                    content.Add(new StringContent(cooktime.ToString()), "CookTime");
                    content.Add(new StringContent(recipe.Serves.ToString()), "Serves");
                    content.Add(new StringContent(recipe.Ingredients), "Ingredients");
                    content.Add(new StringContent(recipe.Directions), "Directions");
                    content.Add(new StringContent(recipe.CategoryID.ToString()), "CategoryID");
                    content.Add(new StringContent(recipe.IsActive.ToString()), "IsActive");
                    var fileContent = new StreamContent(recipe.File.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(recipe.File.ContentType);
                    content.Add(fileContent, "File", recipe.File.FileName);

                    // Gửi yêu cầu đến API
                    var httpResponse = await _httpClient.PostAsync("recipes", content);

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
            var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("RecipeCategory/getallrecipecategory");
            if (response != null)
            {
                ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
            }
            return View(recipe);

        }*/
        public async Task<IActionResult> Edit(int id)
        {
            var httpResponse = await _httpClient.GetAsync($"recipes/foradmin/{id}");
            var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse<JsonElement>>();

            if (data != null && data.Success)
            {
                var preptime = TimeOnly.Parse(data.Data.GetProperty("prepTime").GetString());
                var cooktime = TimeOnly.Parse(data.Data.GetProperty("cookTime").GetString());
                var recipe = new EditRecipe
                {
                    RecipeID = data.Data.GetProperty("recipeID").GetInt32(),
                    Title = data.Data.GetProperty("title").GetString(),
                    PrepHours = preptime.Hour,
                    PrepMinutes = preptime.Minute,
                    CookHours = cooktime.Hour,
                    CookMinutes = cooktime.Minute,
                    Serves = data.Data.GetProperty("serves").GetInt32(),
                    Ingredients = data.Data.GetProperty("ingredients").ToString(),
                    Directions = data.Data.GetProperty("directions").ToString(),
                    CategoryID = data.Data.GetProperty("categoryID").GetInt32()
                };
                HttpContext.Session.SetString("ImageEditRecipe", data.Data.GetProperty("imageURL").ToString());
                ViewBag.CurrentImage = data.Data.GetProperty("imageURL").ToString();
                var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("RecipeCategory/getallrecipecategory");
                if (response != null)
                {
                    ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
                }
                return View(recipe);
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
        public async Task<IActionResult> Edit(EditRecipe edit)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    TimeOnly preptime = new TimeOnly(edit.PrepHours, edit.PrepMinutes);
                    TimeOnly cooktime = new TimeOnly(edit.CookHours, edit.CookMinutes);
                    // Thêm các thông tin khác của Article
                    content.Add(new StringContent(edit.Title), "Title");
                    content.Add(new StringContent(preptime.ToString()), "PrepTime");
                    content.Add(new StringContent(cooktime.ToString()), "CookTime");
                    content.Add(new StringContent(edit.Serves.ToString()), "Serves");
                    content.Add(new StringContent(edit.Ingredients), "Ingredients");
                    content.Add(new StringContent(edit.Directions), "Directions");
                    content.Add(new StringContent(edit.CategoryID.ToString()), "CategoryID");
                    content.Add(new StringContent(edit.IsActive.ToString()), "IsActive");
                    if (edit.File != null && edit.File.Length > 0)
                    {
                        var fileContent = new StreamContent(edit.File.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(edit.File.ContentType);
                        content.Add(fileContent, "File", edit.File.FileName);
                    }

                    // Gửi yêu cầu đến API
                    var httpResponse = await _httpClient.PutAsync($"recipes/{edit.RecipeID}", content);
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
            var response = await _httpClient.GetFromJsonAsync<List<GetCategoryDTO>>("RecipeCategory/getallrecipecategory");
            if (response != null)
            {
                ViewBag.Category = response.Where(x => x.IsDeleted == false).ToList();
            }
            var currentImage = HttpContext.Session.GetString("ImageEditRecipe");
            ViewBag.CurrentImage = currentImage;
            return View(edit);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var httpResponse = await _httpClient.GetAsync($"recipes/foradmin/{id}");
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

        public async Task<IActionResult> Delete(int id)
        {
            var httpResponse = await _httpClient.GetAsync($"recipes/foradmin/{id}");
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
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"recipes/soft/{id}");
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
            string baseUrl = "recipes/foradmin";
            var queryParams = new Dictionary<string, string>();
            queryParams["pageSize"] = pageSize.ToString();
            queryParams["currentPage"] = currentPage.ToString();         
            queryParams["isDeleted"] = true.ToString();
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
                    return RedirectToAction("Index");
                }
            }
            TempData["ErrorMessage"] = "Something went wrong. Please try again.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Restore(int id)
        {
            var httpResponse = await _httpClient.PatchAsync($"recipes/restore/{id}", null);
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
            var httpResponse = await _httpClient.DeleteAsync($"recipes/hard/{id}");
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


        public async Task<IActionResult> Users(string status = "Pending")
        {
            // Dữ liệu công thức

            var responseRecipe = await _httpClient.GetAsync($"recipes/users?status={status}");

            if (responseRecipe.IsSuccessStatusCode)
            {
                var dataRecipes = await responseRecipe.Content.ReadFromJsonAsync<List<JsonElement>>();

                ViewBag.RecipeData = dataRecipes;
            }

            ViewBag.status = status;
            return View();
        }

        public async Task<IActionResult> Approve(int id)
        {
            var newEntity = new
            {
                recipeID= id,
                isApprove= true,
            };
            var response = await _httpClient.PostAsJsonAsync($"recipes/approvals",newEntity);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Update recipe successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Falied to update recipe";   
            }
            return RedirectToAction("Users", new { status = "Approved" });
        }

        public async Task<IActionResult> Reject(int recipeId, string comments)
        {
            var newEntity = new
            {
                recipeID = recipeId,
                isApprove = false,
                comments = comments
            };
            var response = await _httpClient.PostAsJsonAsync($"recipes/approvals", newEntity);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Update recipe successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Falied to update recipe";
            }
            return RedirectToAction("Users", new {status="Rejected"});
        }

        public async Task<IActionResult> DeleteRecipeOfUser(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"recipes/{id}/user");
            if(httpResponse.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Delete recipe successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete recipe";
            }
            return RedirectToAction("Users", new {status="Pending"});
        }
    }
}