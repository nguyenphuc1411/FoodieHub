using System.Net.Http.Json;
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Models.Product;

namespace FoodieHub.MVC.Service.Implementations
{
    public class ProductImageService : IProductImageService
    {
        private readonly HttpClient _httpClient;

        public ProductImageService(IHttpClientFactory httpClientFactory)
        {
            _httpClient= httpClientFactory.CreateClient("MyAPI");
        }


        [HttpPost]
        public async Task<APIResponse> AddImageProduct(ProductImageDTO image)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(image.ProductID.ToString()), "ProductID");

            if (image.ImageURL != null)
            {
                var fileContent = new StreamContent(image.ImageURL.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ImageURL.ContentType);
                content.Add(fileContent, "ImageURL", image.ImageURL.FileName);
            }


            // Gửi yêu cầu POST bất đồng bộ tới API và chờ phản hồi
            var response = await _httpClient.PostAsync($"ProductImage", content);

            // Kiểm tra phản hồi từ API
            if (response.IsSuccessStatusCode)
            {
                // Nếu thành công, đọc và trả về phản hồi dưới dạng APIResponse
                var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse>();
                return apiResponse ?? new APIResponse { Success = false, Message = "No response data from API" };
            }

            // Nếu thất bại, tạo một APIResponse với thông tin lỗi
            return new APIResponse
            {
                Success = false,
                Message = "Failed to add image",
                StatusCode = (int)response.StatusCode
            };
        }




        public async Task<APIResponse> AddMultipleImages(List<ProductImageDTO> images)
        {
            var response = await _httpClient.PostAsJsonAsync($"ProductImages/addmultipleimages", images);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<APIResponse>();
            }
            return new APIResponse
            {
                Success = false,
                Message = "Failed to add multiple images",
                StatusCode = (int)response.StatusCode
            };
        }

        public async Task<APIResponse> DeleteImage(int id)
        {
            var response = await _httpClient.DeleteAsync($"ProductImage/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<APIResponse>();
            }
            return new APIResponse
            {
                Success = false,
                Message = $"Failed to delete image with ID {id}",
                StatusCode = (int)response.StatusCode
            };
        }

        public async Task<List<GetProductImages>> GetAllImg()
        {
            var response = await _httpClient.GetAsync("ProductImage");

            if (response.IsSuccessStatusCode)
            {
                // Đọc JSON từ API và chuyển đổi thành danh sách
                var content = await response.Content.ReadFromJsonAsync<List<GetProductImages>>();
                return content ?? new List<GetProductImages>(); // Trả về danh sách rỗng nếu content là null
            }

            // Log lỗi nếu phản hồi không thành công
            Console.WriteLine($"Failed to retrieve images. Status code: {response.StatusCode}");

            // Trả về danh sách rỗng nếu có lỗi
            return new List<GetProductImages>();
        }


        public async Task<APIResponse> DeleteImgByProductID(int id)
        {
            var response = await _httpClient.DeleteAsync($"ProductImage/deleteimgbyproductid/{id}");

            if (response.IsSuccessStatusCode)
            {
                return new APIResponse
                {
                    Success = true,
                    Message = "Image deleted successfully."
                };
            }

            return new APIResponse
            {
                Success = false,
                Message = "Failed to delete image.",
                StatusCode = (int)response.StatusCode
            };
        }

    }
}
