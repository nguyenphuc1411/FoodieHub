﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Favorite;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
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
        public async Task<IEnumerable<GetArticleDTO>> GetFavoriteArticle()
        {
            var userID = _authService.GetUserID();
            return await _context.Favorites
                .Where(f => f.UserID == userID && f.RecipeID==null)
                .Select(f=>f.Article)
                .ProjectTo<GetArticleDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<IEnumerable<GetRecipeDTO>> GetFavoriteRecipe()
        {
            var userID = _authService.GetUserID();
            return await _context.Favorites
               .Where(f => f.UserID == userID && f.ArticleID == null && f.Recipe != null) 
               .Select(f => f.Recipe) 
               .ProjectTo<GetRecipeDTO>(_mapper.ConfigurationProvider)
               .ToListAsync();
        }
        
        public async Task<Favorite?> Create(CreateFavoriteDTO favorite)
        {
            var userId = _authService.GetUserID();
            var newFavorite = new Favorite
            {
                UserID = userId,
                ArticleID = favorite.ArticleID,
                RecipeID = favorite.RecipeID
            };   
            await _context.Favorites.AddAsync(newFavorite);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? newFavorite : null;
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