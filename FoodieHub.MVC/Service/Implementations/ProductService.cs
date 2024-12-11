using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http.Headers;
using FoodieHub.MVC.Models.Product;

namespace FoodieHub.MVC.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient= httpClientFactory.CreateClient("MyAPI");
        }


        public async Task<List<GetProductDTO>> GetAll()
        {
            var response = await _httpClient.GetAsync("Products");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<GetProductDTO>>();
                return content; // Trả về danh sách rỗng nếu không có dữ liệu
            }

            // Nếu không thành công, trả về danh sách rỗng hoặc có thể ném một ngoại lệ tùy ý bạn
            return new List<GetProductDTO>();
        }


        public async Task<APIResponse<GetProductDTO>> GetById(int id)
        {

            var response = await _httpClient.GetAsync($"Products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<GetProductDTO>>();
                return new APIResponse<GetProductDTO>
                {   
                    Success = content?.Success ?? false,
                    Message = content?.Message ?? "Error retrieving product.",
                    Data = content.Data,
                    StatusCode = (int)response.StatusCode
                };
            }

            // Xử lý khi không thành công
            return new APIResponse<GetProductDTO>
            {
                Success = false,
                Message = "Failed to retrieve product by ID.",
                Data = null,
                StatusCode = (int)response.StatusCode
            };
        }




        public async Task<APIResponse> AddProduct(ProductDTO productDTO)
        {
            var content = new MultipartFormDataContent(); // Tạo một đối tượng MultipartFormDataContent


            content.Add(new StringContent(productDTO.ProductName), "ProductName");
            content.Add(new StringContent(productDTO.Price.ToString()), "Price");
            content.Add(new StringContent(productDTO.Description), "Description");
            content.Add(new StringContent(productDTO.Discount.ToString()), "Discount");
            content.Add(new StringContent(productDTO.StockQuantity.ToString()), "StockQuantity");
            content.Add(new StringContent(productDTO.ShelfLife.ToString()), "ShelfLife");
            content.Add(new StringContent(productDTO.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(productDTO.CategoryID.ToString()), "CategoryID");

            if (productDTO.MainImage != null)
            {
                var fileContent = new StreamContent(productDTO.MainImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(productDTO.MainImage.ContentType);
                content.Add(fileContent, "MainImage", productDTO.MainImage.FileName);
            }

            // Gửi POST request tới API
            var response = await _httpClient.PostAsync($"Products", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;

            }
            else
            {
                // Đọc nội dung phản hồi từ API để lấy thông tin lỗi
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error creating product: {errorMessage}"
                };
            }
        }






        public async Task<APIResponse> UpdateProduct(ProductDTO productDTO)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(productDTO.ProductID.ToString()), "ProductID");
            content.Add(new StringContent(productDTO.ProductName), "ProductName");
            content.Add(new StringContent(productDTO.Price.ToString()), "Price");
            content.Add(new StringContent(productDTO.Description), "Description");
            content.Add(new StringContent(productDTO.Discount.ToString()), "Discount");
            content.Add(new StringContent(productDTO.StockQuantity.ToString()), "StockQuantity");
            content.Add(new StringContent(productDTO.ShelfLife.ToString()), "ShelfLife");
            content.Add(new StringContent(productDTO.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(productDTO.CategoryID.ToString()), "CategoryID");

            // Xử lý trường MainImage
            if (productDTO.MainImage != null && productDTO.MainImage.Length > 0)
            {
                var fileContent = new StreamContent(productDTO.MainImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(productDTO.MainImage.ContentType);
                content.Add(fileContent, "MainImage", productDTO.MainImage.FileName);
            }

            // Gửi PUT request tới API
            var response = await _httpClient.PutAsync("Products", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error updating product: {errorMessage}"
                };
            }
        }



        public async Task<APIResponse> DeleteProduct(int id)
        {
            var response = await _httpClient.DeleteAsync($"Products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var respone = await response.Content.ReadFromJsonAsync<APIResponse>();
                return respone;
            }
            else
            {
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to delete product with ID {id}.",
                    StatusCode = (int)response.StatusCode
                };
            }
        }

        public async Task<APIResponse> SetProductStatusOn(ProductStatusDTO productDTO)
        {


            var jsonContent = JsonContent.Create(productDTO);
            var response = await _httpClient.PutAsJsonAsync("Products/setproductstatus", productDTO);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error updating product: {errorMessage}"
                };
            }
        }

        public async Task<APIResponse> SetProductStatusOff(ProductStatusDTO productDTO)
        {


            var jsonContent = JsonContent.Create(productDTO);
            var response = await _httpClient.PutAsJsonAsync("Products/setproductstatus", productDTO);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error updating product: {errorMessage}"
                };
            }
        }

        public async Task<APIResponse> UpdateProductWithImg(ProductWithImgDTO productDTO)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(productDTO.ProductID.ToString()), "ProductID");
            content.Add(new StringContent(productDTO.ProductName), "ProductName");
            content.Add(new StringContent(productDTO.Price.ToString()), "Price");
            content.Add(new StringContent(productDTO.Description), "Description");
            content.Add(new StringContent(productDTO.Discount.ToString()), "Discount");
            content.Add(new StringContent(productDTO.StockQuantity.ToString()), "StockQuantity");
            content.Add(new StringContent(productDTO.ShelfLife.ToString()), "ShelfLife");
            content.Add(new StringContent(productDTO.CategoryID.ToString()), "CategoryID");

            // Xử lý trường MainImage
            if (productDTO.MainImage != null && productDTO.MainImage.Length > 0)
            {
                var fileContent = new StreamContent(productDTO.MainImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(productDTO.MainImage.ContentType);
                content.Add(fileContent, "MainImage", productDTO.MainImage.FileName);
            }

            // Gửi PUT request tới API
            var response = await _httpClient.PutAsync("Products/updateproductwithimg", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error updating product: {errorMessage}"
                };
            }
        }

        public async Task<APIResponse> UpdateProductNoneImg(ProductNoneImgDTO productDTO)
        {
            var jsonContent = JsonContent.Create(productDTO);
            var response = await _httpClient.PutAsJsonAsync("Products/updateproductnoneimg", productDTO);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error updating product: {errorMessage}"
                };
            }
        }

        public async Task<APIResponse> UpdateProductQuantity(ProductSubtractQuantityDTO productDTO)
        {
            var jsonContent = JsonContent.Create(productDTO);
            var response = await _httpClient.PutAsJsonAsync("Products/updateproductquantity", productDTO);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Error updating product: {errorMessage}"
                };
            }
        }
    }
}
