using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Models.Article;
using System.Text.Json;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Comment;
using FoodieHub.MVC.Models.QueryModel;

namespace FoodieHub.MVC.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IFavoriteService _favoriteService;

        public ArticlesController(
            IArticleService articleService,
            ICommentService commentService,
            IHttpClientFactory httpClientFactory,
            IFavoriteService favoriteService)
        {
            _articleService = articleService;
            _commentService = commentService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index(string? search, int? pageSize, int? currentPage)
        {
            // Gọi API thông qua service
            var articlesFromService = await _articleService.Get(search, pageSize, currentPage);

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



        public async Task<IActionResult> ViewAll(QueryArticleModel queryArticle)
        {
            var result = await _articleService.Get(queryArticle);
            if(result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");
            }
           return View(result);
        }


        public async Task<IActionResult> Detail(int id, string order = "desc")
        {
            var data = await _articleService.GetByID(id);
            if (data == null)
            {
                NotificationHelper.SetErrorNotification(this, "Not found this article");
            }
            ViewBag.UserID = Request.GetCookie("UserID");
            return View(data);
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentDTO comment, string order = "desc")
        {
            if (ModelState.IsValid)
            {
                bool result = await _commentService.Create(comment);
                if(result)
                    NotificationHelper.SetSuccessNotification(this);
                else
                    NotificationHelper.SetErrorNotification(this);
            }
            return RedirectToAction("Detail", new { id = comment.ArticleID, order });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> DeleteComment(int commentID,int articleID ,string order = "desc")
        {
            bool result = await _commentService.Delete(commentID);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id = articleID, order });
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite(int id)
        {
            var favorote = new FavoriteDTO { ArticleID = id };

            bool result = await _favoriteService.Create(favorote);
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);

            return RedirectToAction("Detail", new { id });
        }
       

        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(int id)
        {
            var result = await _favoriteService.Delete(new FavoriteDTO
            {
                ArticleID = id
            });
            if (result)
                NotificationHelper.SetSuccessNotification(this);
            else
                NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Detail", new { id });
        }
    }
}
