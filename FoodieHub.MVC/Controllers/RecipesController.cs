using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.Category;
using FoodieHub.MVC.Models.Ingredient;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.MVC.Helpers;
using Microsoft.AspNetCore.Http;


namespace FoodieHub.MVC.Controllers
{
    public class RecipesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IRecipeService _recipeService;
        private readonly IFavoriteService _favoriteService;

        public RecipesController(IHttpClientFactory httpClientFactory, IRecipeService recipeService, IFavoriteService favoriteService)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
            _recipeService = recipeService;
            _favoriteService = favoriteService;
        }
        [ValidateTokenForUser]
        public IActionResult Create()
        {          
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeDTO recipe)
        {
            if (ModelState.IsValid)
            {
                bool result = await _recipeService.Create(recipe);
                if (result)
                {
                    NotificationHelper.SetSuccessNotification(this);
                    return Redirect("/account/recipe");
                }
                else NotificationHelper.SetErrorNotification(this);               
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
        public async Task<IActionResult> Edit(CreateRecipeDTO edit)
        {
            if (ModelState.IsValid)
            {
               
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
       /* public async Task<IActionResult> Index(string? search, int? pageSize, int? currentPage)
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
*/




        public async Task<IActionResult> ListByCategory(string category)
        {
            // Gọi API lấy danh sách công thức theo danh mục
            var response = await _httpClient.GetFromJsonAsync<List<GetRecipeDTO>>($"recipes/category/{category}");

            if (response != null)
            {
                // Truyền tên danh mục vào ViewData
                ViewData["CategoryName"] = category;
                return View("ListByCategory", response);
            }

            // Nếu không có kết quả, trả về View với danh sách rỗng
            ViewData["CategoryName"] = category;
            return View("ListByCategory", new List<GetRecipeDTO>());
        }


        // Action hiển thị chi tiết bài viết
       /* public async Task<IActionResult> Detail(int id, string order = "desc")
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
*/
        [ValidateTokenForUser]

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var newFavorite = new CreateFavoriteDTO { RecipeID = id };
            bool result = await _favoriteService.Create(newFavorite);
            if (result)
            {
               TempData["SuccessMessage"] = "Favorite successfully";            
            }
            else
            {
               TempData["ErrorMessage"] = "Please try again.";
            }
            return Redirect("/Recipes/Detail/" + id);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            bool result = await _favoriteService.Delete(id);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id });
        } 
        [HttpPost]
        [ValidateTokenForUser]
        public async Task<IActionResult> Rating(int recipeID, int ratingValue)
        {
            var rating = new CreateRatingDTO
            {
                RecipeID = recipeID,
                RatingValue = ratingValue
            };
            bool result = await _recipeService.Rating(rating);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
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
