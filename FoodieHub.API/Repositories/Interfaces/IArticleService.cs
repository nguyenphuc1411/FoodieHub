﻿using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IArticleService
    {
        Task<bool> Create(CreateArticleDTO article);
        Task<bool> Update(UpdateArticleDTO article);
        Task<PaginatedModel<GetArticleDTO>> GetOfUser(QueryModel query,string userID);
        Task<PaginatedModel<GetArticleDTO>> Get(QueryModel query,int? categoryID);
        Task<GetArticleDTO?> GetByID(int id);
        Task<bool> Delete(int id);
    }
}