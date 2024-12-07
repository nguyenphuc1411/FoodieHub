﻿using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Services.Implementations
{
    public class ArticleCategoryService:IArticleCategoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        public ArticleCategoryService(AppDbContext appDbContext, IMapper mapper)
        {

            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ArticleCategory>> GetAllArticleCategories()
        {
            if (_appDbContext == null)
            {
                return new List<ArticleCategory>();
            }
            else
            {
                return await _appDbContext.ArticleCategories.ToListAsync();
            }
        }
        public async Task<ServiceResponse> AddArticleCategory(ArticleCategoryDTO category)
        {
            var obj = _mapper.Map<ArticleCategory>(category);
            _appDbContext.ArticleCategories.Add(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Category added successfully.",
                    Data = _mapper.Map<ArticleCategoryDTO>(obj),
                    StatusCode = 201
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to add category.",
                StatusCode = 400
            };
        }

        public async Task<ServiceResponse> UpdateArticleCategory(ArticleCategoryDTO category)
        {
            var obj = await _appDbContext.ArticleCategories.FindAsync(category.CategoryID);
            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Category not found.",
                    StatusCode = 404
                };
            }
            obj.CategoryName = category.CategoryName;
            obj.IsDeleted = category.IsDeleted;
            _appDbContext.ArticleCategories.Update(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Category updated successfully.",
                    StatusCode = 200
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to update category.",
                StatusCode = 400
            };
        }

        public async Task<ServiceResponse> DeleteArticleCategory(int id)
        {
            var entity = await _appDbContext.ArticleCategories.FindAsync(id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Category not found.",
                    StatusCode = 404
                };
            }

            _appDbContext.ArticleCategories.Remove(entity);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Category deleted successfully.",
                    StatusCode = 200
                };
            }

            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to delete category.",
                StatusCode = 400
            };
        }    
    }
}
