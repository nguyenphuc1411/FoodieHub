using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Comment;
using System.Text.Json;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Response;
using System.Security.Claims;
using Azure.Core;
using FoodieHub.MVC.Service.Implementations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace FoodieHub.MVC.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly HttpClient _httpClient;

        public ArticlesController(
            IArticleService articleService,
            ICommentService commentService,
            IHttpClientFactory httpClientFactory)
        {
            _articleService = articleService;
            _commentService = commentService;
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<IActionResult> Index(string? search, int? pageSize, int? currentPage)
        {
            // Gọi API thông qua service
            var articlesFromService = await _articleService.GetAll(search, pageSize, currentPage);

            // Kiểm tra dữ liệu trả về
            if (articlesFromService == null || !articlesFromService.Any())
            {
                return View(new ArticleViewModel());
            }

            // Feature Article
            var featureArticle = articlesFromService
                .OrderByDescending(a => a.TotalFavorites)
                .ThenBy(a => a.CreatedAt)
                .FirstOrDefault();

            // Top Articles: chỉ chọn bài viết có lượt thích > 0
            var topArticles = articlesFromService
                .Where(a => a.TotalFavorites > 0) // Lọc bài viết có lượt thích
                .OrderByDescending(a => a.TotalFavorites)
                .ThenBy(a => a.CreatedAt)
                .Take(10)
                .ToList();

            // Latest Article
            var latestArticle = articlesFromService
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefault();

            // Latest Articles List
            var latestArticlesList = articlesFromService
                .OrderByDescending(a => a.CreatedAt)
                .Skip(1) // Bỏ qua bài viết đã chọn là Latest Article
                .Take(4)
                .ToList();

            // Tạo ViewModel
            var viewModel = new ArticleViewModel
            {
                FeatureArticle = featureArticle,
                TopArticlesByFavourite = topArticles,
                LatestArticle = latestArticle,
                LatestArticlesList = latestArticlesList
            };

            return View(viewModel);
        }



        public async Task<IActionResult> ViewAll(string? search, int? pageSize = 6, int? currentPage = 1, string sortOrder = "asc", string? category = null)
        {
            var allArticles = await _articleService.GetAll(search, pageSize, currentPage);
            var allCategories = allArticles.Select(a => a.CategoryName).Distinct().ToList();
            var articles = allArticles;

            if (!string.IsNullOrEmpty(category))
            {
                articles = articles.Where(a => a.CategoryName == category).ToList();
            }

            var totalArticles = articles.Count();

            // Sắp xếp
            articles = sortOrder switch
            {
                "asc" => articles.OrderBy(a => a.Title).ToList(),
                "desc" => articles.OrderByDescending(a => a.Title).ToList(),
                "likes" => articles.OrderByDescending(a => a.TotalFavorites).ToList(),
                _ => articles
            };

            var pagedArticles = articles
                .Skip((currentPage.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            ViewData["TotalArticles"] = totalArticles;
            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalArticles / pageSize.Value);
            ViewData["SortOrder"] = sortOrder;
            ViewData["SelectedCategory"] = category;
            ViewData["AllCategories"] = allCategories;

            return View(pagedArticles);
        }


        public async Task<IActionResult> Detail(int id, string order = "desc")
        {
            var articleResponse = await _articleService.GetDetail(id);
            if (!articleResponse.Success || articleResponse.Data == null)
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
                var response = await _httpClient.GetAsync($"Comments/article/{id}?order={order}");
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
            return View(articleResponse.Data);
        }


        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> CreateComment(int articleID, string commentContent, string order = "desc")
        {
            if (string.IsNullOrWhiteSpace(commentContent))
            {
                TempData["ErrorMessage"] = "Comment content cannot be empty.";
                return RedirectToAction("Detail", new { id = articleID, order });
            }

            var response = await _commentService.CreateArticleCommentAsync(new CreateArticleComment
            {
                ArticleID = articleID,
                CommentContent = commentContent
            });

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "An error occurred while posting the comment. Please try again.";
            }
            else
            {
                TempData["SuccessMessage"] = "Your comment has been posted successfully.";
            }

            return RedirectToAction("Detail", new { id = articleID, order });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> DeleteComment(int commentID, int articleID, string type = "article", string order = "desc")
        {
            var response = await _commentService.DeleteCommentAsync(commentID, type);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the comment.";
            }
            else
            {
                TempData["SuccessMessage"] = "Comment deleted successfully.";
            }

            return RedirectToAction("Detail", new { id = articleID, order });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var content = new StringContent(JsonSerializer.Serialize(id), System.Text.Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync("Favorites/article", content);

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
       

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            var httpResponse = await _httpClient.DeleteAsync($"Favorites/unfa/{id}");

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
    }
}
