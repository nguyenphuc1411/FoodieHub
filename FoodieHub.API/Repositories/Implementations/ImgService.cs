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
    public class ImgService:IImgService
    {
         private readonly AppDbContext _appDbContext;
        IMapper _mapper;
        ImageExtentions _uploadImageHelper;
        public ImgService(AppDbContext appDbContext, IMapper mapper, ImageExtentions uploadImageHelper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _uploadImageHelper = uploadImageHelper;
        }


        public async Task<ServiceResponse> AddImage(ProductImageDTO img)
        {
            var uploadImageResult = await _uploadImageHelper.UploadImage(img.ImageURL, "Products");

            if (!uploadImageResult.Success)
            {
                return new ServiceResponse
                {
                    Success =false,
                    Message = "Failed to upload img to folder.",
                    StatusCode = 201

                };
            }
           
            var obj = _mapper.Map<ProductImage>(img);
            obj.ProductID =img.ProductID;
            obj.ImageURL = uploadImageResult.FilePath.ToString();
            await _appDbContext.AddAsync(obj);
            var result = await _appDbContext.SaveChangesAsync();
            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Add new img successfully.",
                    StatusCode = 201
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to add new img.",
                    StatusCode = 400

                };
            }
        }

        public async Task<ServiceResponse> AddMultipleImages(List<ProductImageDTO> imgs)
        {
            var uploadedImages = new List<ProductImageDTO>();

            foreach (var img in imgs)
            {
                
                var uploadImageResult = await _uploadImageHelper.UploadMutipleImages((List<IFormFile>)img.ImageURL, "Products");

                if (!uploadImageResult.Success)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = $"Failed to upload image for index {imgs.IndexOf(img)}.",
                        StatusCode = 201
                    };
                }

                // Chuyển đổi và lưu thông tin hình ảnh vào cơ sở dữ liệu
                var obj = _mapper.Map<ProductImage>(img);
                obj.ProductID = img.ProductID;
                obj.ImageURL = uploadImageResult.FilePath.ToString(); 

                await _appDbContext.AddAsync(obj);
                uploadedImages.Add(_mapper.Map<ProductImageDTO>(obj)); 
            }

            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Added new images successfully.",
                    StatusCode = 201
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to add new images.",
                    StatusCode = 400
                };
            }
        }

        public async Task<ServiceResponse> DeleteImgByProductID(int id)
        {
            if (_appDbContext == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Database context is not available.",
                    StatusCode = 500
                };
            }

            // Lấy danh sách các ảnh có ProductID tương ứng
            var images = await _appDbContext.ProductImages
                                            .Where(img => img.ProductID == id)
                                            .ToListAsync();

            if (images == null || images.Count == 0)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"No images found for ProductID: {id}",
                    StatusCode = 404
                };
            }

            // Xóa các ảnh trong danh sách
            _appDbContext.ProductImages.RemoveRange(images);
            await _appDbContext.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "Images deleted successfully.",
                StatusCode = 200
            };
        }


        public async Task<ServiceResponse> DeleteImage(int id)
        {
            var obj = await _appDbContext.ProductImages.FindAsync(id);
            if (obj == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Can't find image with ID: {id}",
                    StatusCode = 404
                };
            }

             _uploadImageHelper.DeleteImage(obj.ImageURL);
          

            _appDbContext.ProductImages.Remove(obj);

            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Image deleted successfully.",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to delete image record from database.",
                    StatusCode = 500
                };
            }
        }

        public async Task<List<ProductImage>> GetAllImg()
        {
            if (_appDbContext == null)
            {
                return null;
            }

            var imgs = await _appDbContext.ProductImages.ToListAsync();
            return imgs;
        }


    }
}
