using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Models.CommentRecipe;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using FoodieHub.MVC.Models.Category;
using FoodieHub.MVC.Models.Ingredient;


namespace FoodieHub.MVC.Controllers
{
    public class RecipesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IRecipeService _recipeService;
        private readonly IRecipeCommentService _recipeCommentService;

        public RecipesController(IHttpClientFactory httpClientFactory, IRecipeService recipeService, IRecipeCommentService recipeCommentService)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
            _recipeService = recipeService;
            _recipeCommentService = recipeCommentService;
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Create()
        {
            var response = await _httpClient.GetFromJsonAsync<List<CategoryDTO>>("RecipeCategory/getallrecipecategory");
            if (response != null)
            {
                ViewBag.Category = response.ToList();
            }
            var responseIngredients = await _httpClient.GetFromJsonAsync<List<IngredientDTO>>("ingredients");
            if(responseIngredients!=null)
            {
                ViewBag.Ingredients = responseIngredients;
            }


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipe)
        {
            // Kiểm tra ModelState hợp lệ trước khi tiếp tục
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các thông tin khác của Recipe
                    content.Add(new StringContent(recipe.Title), "Title");
                    content.Add(new StringContent(recipe.CookTime.ToString()), "CookTime");
                    content.Add(new StringContent(recipe.Serves.ToString()), "Serves");
                    content.Add(new StringContent(recipe.CategoryID.ToString()), "CategoryID");
                    content.Add(new StringContent(recipe.IsActive.ToString()), "IsActive");
                    content.Add(new StringContent(recipe.Description ?? string.Empty), "Description");

                    // Thêm file chính
                    if (recipe.File != null)
                    {
                        var fileContent = new StreamContent(recipe.File.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(recipe.File.ContentType);
                        content.Add(fileContent, "File", recipe.File.FileName);
                    }

                    // Sử lý các bước (RecipeSteps)
                    for (int i = 0; i < recipe.RecipeSteps.Count; i++)
                    {
                        content.Add(new StringContent(recipe.RecipeSteps[i].Step.ToString()), $"RecipeSteps[{i}].Step");
                        content.Add(new StringContent(recipe.RecipeSteps[i].Directions), $"RecipeSteps[{i}].Directions");

                        if (recipe.RecipeSteps[i].ImageStep != null)
                        {
                            var fileContentStep = new StreamContent(recipe.RecipeSteps[i].ImageStep.OpenReadStream());
                            fileContentStep.Headers.ContentType = new MediaTypeHeaderValue(recipe.RecipeSteps[i].ImageStep.ContentType);
                            content.Add(fileContentStep, $"RecipeSteps[{i}].ImageStep", recipe.RecipeSteps[i].ImageStep.FileName);
                        }
                    }

                    // Sử lý nguyên liệu (Ingredients)
                    for (int i = 0; i < recipe.Ingredients.Count; i++)
                    {
                        content.Add(new StringContent(recipe.Ingredients[i].Name), $"Ingredients[{i}].Name");
                        content.Add(new StringContent(recipe.Ingredients[i].Quantity.ToString()), $"Ingredients[{i}].Quantity");
                        content.Add(new StringContent(recipe.Ingredients[i].Unit), $"Ingredients[{i}].Unit");

                        if (recipe.Ingredients[i].ProductID.HasValue)
                        {
                            content.Add(new StringContent(recipe.Ingredients[i].ProductID.Value.ToString()), $"Ingredients[{i}].ProductID");
                        }
                    }

                    // Gửi yêu cầu HTTP POST
                    var httpResponse = await _httpClient.PostAsync("recipes", content);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        return Redirect("/account/recipe");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred. Please try again.";
                    }
                }
            }

            // Lấy dữ liệu Category và trả lại view nếu ModelState không hợp lệ
            var response = await _httpClient.GetFromJsonAsync<List<CategoryDTO>>("RecipeCategory/getallrecipecategory");
            if (response != null)
            {
                ViewBag.Category = response.ToList();
            }
            return View(recipe);
        }




        /*    public async Task<IActionResult> Edit(int id)
            {
                var responseCategory = await _httpClient.GetFromJsonAsync<List<CategoryDTO>>("RecipeCategory/getallrecipecategory");
                if (responseCategory != null)
                {
                    ViewBag.Category = responseCategory.ToList();
                }


                var response = await _httpClient.GetAsync("recipes/edit/" + id);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var preptime = TimeOnly.Parse(data.GetProperty("prepTime").GetString());
                    var cooktime = TimeOnly.Parse(data.GetProperty("cookTime").GetString());
                    var recipe = new EditRecipe
                    {
                        RecipeID = data.GetProperty("recipeID").GetInt32(),
                        Title = data.GetProperty("title").GetString(),
                        PrepHours = preptime.Hour,
                        PrepMinutes = preptime.Minute,
                        CookHours = cooktime.Hour,
                        CookMinutes = cooktime.Minute,
                        ImageURL = data.GetProperty("imageURL").ToString(),
                        Serves = data.GetProperty("serves").GetInt32(),
                        Ingredients = data.GetProperty("ingredients").ToString(),
                        Directions = data.GetProperty("directions").ToString(),
                        CategoryID = data.GetProperty("categoryID").GetInt32()
                    };
                    return View(recipe);
                }

                return RedirectToAction("RecipesAndFollows", "Account");
            }*/

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
                            return RedirectToAction("RecipesAndFollows", "Account");
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
            return View(edit);
        }


        // Action hiển thị danh sách bài viết
        //public async Task<IActionResult> Index(string? search, int? pageSize, int? currentPage)
        //{
        //    // Lấy danh sách công thức
        //    var recipes = await _recipeService.GetRecipes(search, pageSize, currentPage);

        //    // Lấy danh sách danh mục từ API
        //    var categoryResponse = await _httpClient.GetFromJsonAsync<List<CategoryDTO>>("RecipeCategory/getallrecipecategory");
        //    var categories = categoryResponse?.Select(c => new
        //    {
        //        c.CategoryName,

        //    }).ToList();

        //    ViewData["Categories"] = categories;

        //    return View(recipes);
        //}
        public async Task<IActionResult> Index(string? search, int? pageSize, int? currentPage)
        {
            // Lấy danh sách công thức
            var recipes = await _recipeService.GetRecipes(search, pageSize, currentPage);

            // Lấy danh sách danh mục từ API
            var categoryResponse = await _httpClient.GetFromJsonAsync<List<CategoryDTO>>("RecipeCategory/getallrecipecategory");
            var categories = categoryResponse?.Select(c => c.CategoryName).ToList(); // Chỉ lấy tên danh mục

            // Truyền danh mục qua ViewData
            ViewData["Categories"] = categories;

            return View(recipes);
        }





        public async Task<IActionResult> ListByCategory(string category)
        {
            // Gọi API lấy danh sách công thức theo danh mục
            var response = await _httpClient.GetFromJsonAsync<List<GetRecipes>>($"recipes/category/{category}");

            if (response != null)
            {
                // Truyền tên danh mục vào ViewData
                ViewData["CategoryName"] = category;
                return View("ListByCategory", response);
            }

            // Nếu không có kết quả, trả về View với danh sách rỗng
            ViewData["CategoryName"] = category;
            return View("ListByCategory", new List<GetRecipes>());
        }


        // Action hiển thị chi tiết bài viết
        public async Task<IActionResult> Detail(int id, string order = "desc")
        {
            var recipeResponse = await _recipeService.GetRecipeDetail(id);
            if (!recipeResponse.Success || recipeResponse.Data == null)
            {
                return NotFound();
            }

            var tokenUser = Request.Cookies["TokenUser"];
            string userId = null;

            if (!string.IsNullOrEmpty(tokenUser))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(tokenUser) as JwtSecurityToken;
                    if (jsonToken != null)
                    {
                        // Lấy userId từ token
                        userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error decoding token: {ex.Message}");
                }
            }

            try
            {
                var response = await _httpClient.GetAsync($"Comments/recipe/{id}?order={order}");
                var content = await response.Content.ReadAsStringAsync();
                var commentData = JsonDocument.Parse(content);

                if (commentData.RootElement.GetProperty("success").GetBoolean())
                {
                    var comments = commentData.RootElement
                        .GetProperty("data")
                        .EnumerateArray()
                        .Select(item => new GetComment
                        {
                            CommentID = item.GetProperty("commentID").GetInt32(),
                            CommentContent = item.GetProperty("commentContent").GetString(),
                            Avatar = item.GetProperty("avatar").ValueKind == JsonValueKind.Null
                                ? null
                                : item.GetProperty("avatar").GetString(),
                            FullNameComment = item.GetProperty("fullname").GetString(),
                            CommentAt = item.GetProperty("commentedAt").GetDateTime(),
                            UserID = item.GetProperty("userID").GetString(),
                            ParentCommentID = null
                        })
                        .ToList();
                    ViewBag.TotalComment = comments.Count; // Đếm tổng số bình luận

                    // Lấy tên người dùng hiện tại nếu có trong bình luận
                    var currentUserComment = comments.FirstOrDefault(c => c.UserID == userId);
                    ViewBag.CurrentUserFullName = currentUserComment?.FullNameComment;

                    comments = order == "asc"
                        ? comments.OrderBy(c => c.CommentAt).ToList()
                        : comments.OrderByDescending(c => c.CommentAt).ToList();

                    ViewBag.Comments = comments;
                    ViewBag.UserID = userId;
                }
            }
            catch (Exception)
            {
                ViewBag.Comments = null;
                ViewBag.TotalComment = 0; // Trong trường hợp lỗi, đặt TotalComment là 0
            }

            ViewBag.Order = order;
            ViewBag.ArticleID = id;
            return View(recipeResponse.Data);
        }

        [ValidateTokenForUser]

        [HttpPost]
        public async Task<IActionResult> CreateComment(int recipeID, string commentContent, string order = "desc")
        {
            if (string.IsNullOrWhiteSpace(commentContent))
            {
                TempData["ErrorMessage"] = "Comment content cannot be empty.";
                return RedirectToAction("Detail", new { id = recipeID, order });
            }

            var response = await _recipeCommentService.CreateRecipeCommentAsync(new CreateRecipeComment
            {
                RecipeID = recipeID,
                CommentContent = commentContent
            });

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting the comment. Please try again.";
            }
            else
            {
                TempData["SuccessMessage"] = "Comment has been successfully submitted.";
            }

            return RedirectToAction("Detail", new { id = recipeID, order });
        }





        [ValidateTokenForUser]
        public async Task<IActionResult> DeleteComment(int id, string type = "RECIPE")
        {
            var response = await _recipeCommentService.DeleteCommentAsync(id, type);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the comment.";
            }
            else
            {
                TempData["SuccessMessage"] = "Comment deleted successfully.";
            }

            return RedirectToAction("Detail", new { id = id });
        }





        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync($"Favorites/recipe",id);
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
            return Redirect("/Recipes/Detail/" + id);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"Favorites/unfr/{id}");

            if (httpResponse.IsSuccessStatusCode)
            {
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
                    TempData["ErrorMessage"] = "Response does not contain valid JSON data.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"Error calling API: {httpResponse.StatusCode}";
            }

            return RedirectToAction("Detail", new { id });
        }



        public async Task<IActionResult> NewestRecipes()
        {
            // Adjust `pageSize` and `currentPage` as needed
            int pageSize = 4;
            int currentPage = 1;

            var newestRecipes = await _recipeService.GetRecipes(null, pageSize, currentPage);

            // Sort by `CreateAt` if not handled in `GetRecipes` itself
            newestRecipes = newestRecipes.OrderByDescending(r => r.CreatedAt);

            return View(newestRecipes);
        }


        public async Task<IActionResult> ViewAll(string? search, int? pageSize = 6, int? currentPage = 1, string sortOrder = "asc", string? category = null)
        {
            // Fetch all recipes, then apply search, sort, and filter criteria
            var allRecipes = await _recipeService.GetRecipes(search, null, null);

            // Get distinct categories from all recipes (not filtered)
            var allCategories = allRecipes.Select(a => a.CategoryName).Distinct().ToList();

            // Filter by category if specified
            var filteredRecipes = allRecipes;
            if (!string.IsNullOrEmpty(category))
            {
                filteredRecipes = filteredRecipes.Where(a => a.CategoryName == category).ToList();
            }

            // Total count after filtering
            var totalRecipes = filteredRecipes.Count();

            // Sort recipes by title in specified order
            filteredRecipes = sortOrder == "asc"
                ? filteredRecipes.OrderBy(a => a.Title).ToList()
                : filteredRecipes.OrderByDescending(a => a.Title).ToList();

            // Pagination
            int pageSizeValue = pageSize ?? 6;
            int currentPageValue = currentPage ?? 1;
            var pagedRecipes = filteredRecipes
                .Skip((currentPageValue - 1) * pageSizeValue)
                .Take(pageSizeValue)
                .ToList();

            // Pass data to ViewData for the view
            ViewData["TotalRecipes"] = totalRecipes;
            ViewData["PageSize"] = pageSizeValue;
            ViewData["CurrentPage"] = currentPageValue;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalRecipes / pageSizeValue);
            ViewData["SortOrder"] = sortOrder;
            ViewData["SelectedCategory"] = category;
            ViewData["AllCategories"] = allCategories;

            return View(pagedRecipes);
        }

        public async Task<IActionResult> Search(string? search)
        {
            var recipes = await _recipeService.GetRecipes(search, null, null);
            ViewData["SearchQuery"] = search; // Lưu từ khóa tìm kiếm để hiển thị trên view
            return View("Search", recipes); // Truyền danh sách vào view
        }

        [HttpPost]
        [ValidateTokenForUser]
        public async Task<IActionResult> Rating(int recipeID, int ratingValue)
        {
            // Tạo dữ liệu payload cho yêu cầu POST
            var ratingData = new
            {
                RecipeID = recipeID,
                RatingValue = ratingValue
            };

            // Gửi yêu cầu POST tới API
            var httpResponse = await _httpClient.PostAsJsonAsync("Recipes/rating", ratingData);

            // Đọc phản hồi từ API
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

            // Chuyển hướng về trang chi tiết công thức
            return Redirect("/Recipes/Detail/" + recipeID);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync("recipes/" + id);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Delete Recipe Successfully";              
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete recipe";
            }        
            return RedirectToAction("RecipesAndFollows", "Account");
        }

    }
}
