using AutoMapper;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Data;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Services.Interfaces;
using FoodieHub.API.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Services.Implementations
{
    public class RecipeCategoryService:IRecipeCategoryService
    {
        private readonly AppDbContext _appDbContext;
        IMapper _mapper;
        ImageExtentions _uploadImageHelper;
        public RecipeCategoryService(AppDbContext appDbContext, IMapper mapper, ImageExtentions uploadImageHelper)
        {
            _uploadImageHelper = uploadImageHelper;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeCategory>> GetAllRecipeCategories()
        {
            if (_appDbContext == null)
            {
                return Enumerable.Empty<RecipeCategory>();
            }
            else
            {
                return await _appDbContext.RecipeCategories.ToListAsync();
            }
        }

        public async Task<ServiceResponse> AddRecipeCategory(RecipeCategoryDTO category)
        {
            var uploadImageResult = await _uploadImageHelper.UploadImage(category.ImageURL, "Images");

            if (!uploadImageResult.Success)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to upload img to folder.",
                    StatusCode = 201

                };
            }
            var obj = _mapper.Map<RecipeCategory>(category);
            obj.ImageURL = uploadImageResult.FilePath.ToString();
            _appDbContext.RecipeCategories.Add(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Category added successfully.",
                    Data = obj.CategoryID,
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

        public async Task<ServiceResponse> UpdateRecipeCategoryWithImg(RecipeCategoryWithImgDTO category)
        {
            var obj = await _appDbContext.RecipeCategories.FindAsync(category.CategoryID);
            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Category not found.",
                    StatusCode = 404
                };
            }
            var uploadImageResult = await _uploadImageHelper.UploadImage(category.ImageURL, "Images");

            if (!uploadImageResult.Success)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to upload img to folder.",
                    StatusCode = 201

                };
            }
            obj.CategoryName = category.CategoryName;
            obj.ImageURL = uploadImageResult.FilePath.ToString();
            _appDbContext.RecipeCategories.Update(obj);
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

        public async Task<ServiceResponse> UpdateRecipeCategoryNoneImg(RecipeCategoryNoneImgDTO category)
        {
            var obj = await _appDbContext.RecipeCategories.FindAsync(category.CategoryID);
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
            _appDbContext.RecipeCategories.Update(obj);
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

       

        public async Task<ServiceResponse> DeleteRecipeCategory(int id)
        {
            var entity = await _appDbContext.RecipeCategories.FindAsync(id);
            if (entity == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Category not found.",
                    StatusCode = 404
                };
            }

            _appDbContext.RecipeCategories.Remove(entity);
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

        public async Task<ServiceResponse> UpdateStatusCategory(RecipeCategoryStatusDTO category)
        {
            var obj = await _appDbContext.RecipeCategories.FindAsync(category.CategoryID);
            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Category not found.",
                    StatusCode = 404
                };
            }
            obj.IsActice = category.IsActice;
            obj.IsDeleted = category.IsDeleted;
            _appDbContext.RecipeCategories.Update(obj);
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
    }
}
