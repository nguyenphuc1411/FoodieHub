using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public FavoriteService(IAuthService authService, AppDbContext context, IMapper mapper)
        {
            _authService = authService;
            _context = context;
            _mapper = mapper;
        }

       
        public async Task<ServiceResponse> GetFavoriteArticle()
        {
            var userID = _authService.GetUserID();

            var listFavoriteArticle = await _context.Favorites
                .Where(fa => fa.UserID == userID)
                .Join(_context.Articles,
                      favorite => favorite.ArticleID,
                      article => article.ArticleID,
                      (favorite, article) => article)
                .ProjectTo<GetArticles>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Get favorite articles successfully",
                Data = listFavoriteArticle,
                StatusCode = 200
            };
        }
       
        public async Task<ServiceResponse> GetFavoriteRecipe()
        {
            var userID = _authService.GetUserID();

            var listFavoriteRecipe = await _context.Favorites
                .Where(fa => fa.UserID == userID)
                .Join(_context.Recipes,
                  favorite => favorite.RecipeID,
                  recipe => recipe.RecipeID,
                  (favorite, recipe) => recipe)
                .ProjectTo<GetRecipes>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Get favorite articles successfully",
                Data = listFavoriteRecipe,
                StatusCode = 200
            };
        }
        public async Task<ServiceResponse> GetArticleRanking()
        {
            var articleRanking = await _context.Favorites
                .GroupBy(fa => fa.ArticleID) // Nhóm theo ArticleID
                .Select(group => new
                {
                    ArticleID = group.Key,
                    FavoriteCount = group.Count() // Đếm số lượng yêu thích cho mỗi bài viết
                })
                .Join(_context.Articles,
                      ranking => ranking.ArticleID,
                      article => article.ArticleID,
                      
                      (ranking, article) => new
                      {
                          
                          UpdatedAt = DateTime.Now,
                          article.Title, // Hoặc các thông tin khác về bài viết
                          article.ArticleID,
                          ranking.FavoriteCount
                      })
                .OrderByDescending(ranking => ranking.FavoriteCount) // Sắp xếp giảm dần theo số lượng yêu thích
                .ToListAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Get article ranking successfully",
                Data = articleRanking,
                StatusCode = 200
            };
        }

        public async Task<Favorite?> Create(Favorite favorite)
        {
            var userId = _authService.GetUserID();
            favorite.UserID = userId;       
            await _context.Favorites.AddAsync(favorite);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? favorite : null;
        }

        public async Task<bool> Delete(int id)
        {
            var entityToDelete = await _context.Favorites.FindAsync(id);
            if (entityToDelete == null) return false;
            var userID = _authService.GetUserID();
            if(entityToDelete.UserID != userID) return false;
            _context.Favorites.Remove(entityToDelete);
            return await _context.SaveChangesAsync()>0;
        }
    }
}
