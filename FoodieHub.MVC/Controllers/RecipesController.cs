using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.QueryModel;

namespace FoodieHub.MVC.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICommentService _commentService;
        public RecipesController(IRecipeService recipeService, IFavoriteService favoriteService, ICommentService commentService)
        {
            _recipeService = recipeService;
            _favoriteService = favoriteService;
            _commentService = commentService;
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
                    return Redirect("/account/recipes");
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
        public IActionResult Edit(CreateRecipeDTO edit)
        {
            if (ModelState.IsValid)
            {
               
            }
            return View(edit);
        }


        public async Task<IActionResult> Index(QueryRecipeModel query)
        {
            // Lấy danh sách công thức
            var recipes = await _recipeService.GetAll(query);
            return View(recipes);
        }



        // Action hiển thị chi tiết bài viết
        public async Task<IActionResult> Detail(int id)
        {
            var data = await _recipeService.GetByID(id);
            if (data == null)
            {
                NotificationHelper.SetErrorNotification(this,"Not found this recipe");
            }
            ViewBag.UserID = Request.GetCookie("UserID");
            return View(data);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var newFavorite = new FavoriteDTO { RecipeID = id };
            bool result = await _favoriteService.Create(newFavorite);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return Redirect("/Recipes/Detail/" + id);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            bool result = await _favoriteService.Delete(new FavoriteDTO
            {
                RecipeID = id
            });
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id });
        } 
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
            var result = await _recipeService.Delete(id);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Recipes", "Account");
        }

        public async Task<IActionResult> CreateComment(CommentDTO comment)
        {
            bool result = await _commentService.Create(comment);
            if(result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = comment.RecipeID});
        }

        public async Task<IActionResult> DeleteComment(int id,int recipeID)
        {
            bool result = await _commentService.Delete(id);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = recipeID });
        }

        public async Task<IActionResult> EditComment(int CommentID,string CommentContent,int RecipeID)
        {
            bool result = await _commentService.Edit(CommentID,new CommentDTO
            {
                CommentContent = CommentContent
            });
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = RecipeID });
        }
    }
}
