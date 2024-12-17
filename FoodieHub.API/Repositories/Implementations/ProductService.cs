using AutoMapper;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Data;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Repositories.Interfaces;
using FoodieHub.API.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class ProductService: IProductService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ImageExtentions _uploadImageHelper;


        private IMapper _mapper;

        public ProductService(AppDbContext appDbContext, IMapper mapper, ImageExtentions uploadImageHelper)
        {
            _uploadImageHelper = uploadImageHelper;
            _appDbContext = appDbContext;
            _mapper = mapper;

        }
        public async Task<List<Product>> GetProducts()
        {
            if (_appDbContext == null)
            {
                return [];
            }
            else
            {
                var products = await _appDbContext.Products.ToListAsync();
                return products;
            }
        }


        public async Task<ServiceResponse> GetProductByID(int id)
        {
            var obj = await _appDbContext.Products.FindAsync(id);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID: {id}",
                    StatusCode = 404
                };
            }

            return new ServiceResponse
            {
                Success = true,
                StatusCode = 200,
                Data = obj
            };
        }

        public async Task<ServiceResponse> GetProductByName(string name)
        {
            var obj = await _appDbContext.Products.FirstOrDefaultAsync(x => x.ProductName == name);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with Name: {name}",
                    StatusCode = 404
                };
            }

            return new ServiceResponse
            {
                Success = true,
                Data = _mapper.Map<ProductDTO>(obj),
                StatusCode = 200
            };
        }
        public async Task<ServiceResponse> AddProduct(ProductDTO product)
        {
            var obj = _mapper.Map<Product>(product);

            var existName = _appDbContext.Products.Any(x => x.ProductName == product.ProductName);

            if (existName) {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Name is already exist! Please choose another name.",
                    Data = obj.ProductID,
                    StatusCode = 201
                };
            }
         


            var uploadResult = await _uploadImageHelper.UploadImage(product.MainImage, "Products");

            if (!uploadResult.Success)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = uploadResult.Message,

                    StatusCode = 400
                };
            }

            obj.MainImage = uploadResult.FilePath.ToString();

            _appDbContext.Products.Add(obj);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Add new product successfully.",
                    Data = obj.ProductID,
                    StatusCode = 201
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to add new product.",
                    StatusCode = 400
                };
            }
        }



        public async Task<ServiceResponse> UpdateProduct(ProductDTO productDto)
        {
            var obj = await _appDbContext.Products.FindAsync(productDto.ProductID);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID {productDto.ProductID}",
                    StatusCode = 404
                };
            }

           

            obj.ProductName = productDto.ProductName;
            obj.Price = productDto.Price;
            obj.Description = productDto.Description;
            obj.ShelfLife = productDto.ShelfLife;
            obj.CategoryID = productDto.CategoryID;
            var uploadResult = await _uploadImageHelper.UploadImage(productDto.MainImage, "Products");

            if (!uploadResult.Success)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = uploadResult.Message,

                    StatusCode = 400
                };
            }

            obj.MainImage = uploadResult.FilePath.ToString();
            obj.Discount = productDto.Discount;
            obj.StockQuantity = productDto.StockQuantity;
            obj.IsActive = productDto.IsActive;


            _appDbContext.Products.Update(obj);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = $"Product with ID {productDto.ProductID} has been updated successfully.",
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> DeleteProduct(int id)
        {
            var obj = await _appDbContext.Products.FindAsync(id);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID: {id}",
                    StatusCode = 404
                };
            }

            _appDbContext.Products.Remove(obj);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = $"Product with ID {id} has been deleted successfully.",
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> SetProductStatus(ProductStatusDTO productDto)
        {
            var obj = await _appDbContext.Products.FindAsync(productDto.ProductID);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID {productDto.ProductID}",
                    StatusCode = 404
                };
            }

            obj.IsActive = productDto.IsActive;


            _appDbContext.Products.Update(obj);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = $"Product with ID {productDto.ProductID} has been updated successfully.",
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> UpdateProductWithImg(ProductWithImgDTO product)
        {
            var obj = await _appDbContext.Products.FindAsync(product.ProductID);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID {product.ProductID}",
                    StatusCode = 404
                };
            }

          

            obj.ProductName = product.ProductName;
            obj.Price = product.Price;
            obj.Description = product.Description;
            obj.ShelfLife = product.ShelfLife;
            obj.CategoryID = product.CategoryID;
            var uploadResult = await _uploadImageHelper.UploadImage(product.MainImage, "Products");

            if (!uploadResult.Success)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = uploadResult.Message,

                    StatusCode = 400
                };
            }

            obj.MainImage = uploadResult.FilePath.ToString();
            obj.Discount = product.Discount;
            obj.StockQuantity = product.StockQuantity;


            _appDbContext.Products.Update(obj);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = $"Product with ID {product.ProductID} has been updated successfully.",
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> UpdateProductNoneImg(ProductNoneImgDTO product)
        {
            var obj = await _appDbContext.Products.FindAsync(product.ProductID);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID {product.ProductID}",
                    StatusCode = 404
                };
            }
            var existName = _appDbContext.Products.Any(x => x.ProductName == product.ProductName);

          
            obj.ProductName = product.ProductName;
            obj.Price = product.Price;
            obj.Description = product.Description;
            obj.CategoryID = product.CategoryID;
            obj.Discount = product.Discount;
            obj.ShelfLife = product.ShelfLife;
            obj.StockQuantity = product.StockQuantity;


            _appDbContext.Products.Update(obj);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = $"Product with ID {product.ProductID} has been updated successfully.",
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> UpdateStockQuantity(ProductSubtractQuantityDTO product)
        {
            var obj = await _appDbContext.Products.FindAsync(product.ProductID);

            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find product with ID {product.ProductID}",
                    StatusCode = 404
                };
            }
            obj.StockQuantity = product.StockQuantity;

            _appDbContext.Products.Update(obj);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = $"Product with ID {product.ProductID} has been updated successfully.",
                StatusCode = 200
            };
        }
    }
}
