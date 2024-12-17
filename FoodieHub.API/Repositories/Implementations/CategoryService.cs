using AutoMapper;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Data;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Repositories.Interfaces;
using FoodieHub.API.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class CategoryService: ICategoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        public CategoryService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            if (_appDbContext == null)
            {
                return Enumerable.Empty<Category>();
            }
            else
            {
                return await _appDbContext.Categories.ToListAsync();
            }
        }
        public async Task<ServiceResponse> AddCategory(CategoryDTO category)
        {
            var obj = _mapper.Map<Category>(category);

            var existName = _appDbContext.  Categories.Any(x => x.CategoryName == category.CategoryName);

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
            _appDbContext.Categories.Add(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Add new category successfully.",
                    Data = _mapper.Map<CategoryDTO>(obj),
                    StatusCode = 201

                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to add new category.",
                    StatusCode = 400

                };
            }


        }
        public async Task<ServiceResponse> UpdateCategory(CategoryDTO category)
        {
            var obj = await _appDbContext.Categories.FindAsync(category.CategoryID);
            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find category with ID {category.CategoryID}",
                    StatusCode = 404
                };
            }
          
            obj.CategoryName = category.CategoryName;
            _appDbContext.Categories.Update(obj);
            var result = await _appDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Update category successfully.",
                    Data = _mapper.Map<CategoryDTO>(obj),
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to update category.",
                    StatusCode = 500
                };
            }

        }
        public async Task<ServiceResponse> DeleteCategory(int categoryId)
        {
            var obj = await _appDbContext.Categories.FindAsync(categoryId);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find category with ID {categoryId}.",
                    StatusCode = 404
                };
            }

            _appDbContext.Categories.Remove(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Category deleted successfully.",
                    Data = _mapper.Map<CategoryDTO>(obj),
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to delete category.",
                    StatusCode = 500
                };
            }
        }

        public async Task<CategoryDTO> GetCategoryById(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);


            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return _mapper.Map<CategoryDTO>(category);
        }

    }
}
