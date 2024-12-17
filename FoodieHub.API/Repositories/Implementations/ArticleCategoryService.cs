using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
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

            var existName = _appDbContext.ArticleCategories.Any(x => x.CategoryName == category.CategoryName);

            if (existName)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Name is already exist! Please choose another name.",
                    Data = obj.CategoryName,
                    StatusCode = 201
                };
            }
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
