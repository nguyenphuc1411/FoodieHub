using AutoMapper;
using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    // API quản lý sản phẩm bao gồm sản phẩm, hình ảnh sản phẩm, các đánh giá
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IImgService _imgService;
        IMapper _mapper;
        public ProductsController(IProductService service, IImgService imgService, IMapper mapper)
        {
            _service = service;
            _imgService = imgService;
            _mapper = mapper;
        }

        [HttpGet("getallproducts")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetProducts();
            return Ok(result);

        }

        [HttpGet("getproductbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetProductByID(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("getproductbyname/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _service.GetProductByName(name);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("addproduct")]
        public async Task<IActionResult> Post([FromForm] ProductDTO product)
        {
            var result = await _service.AddProduct(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updateproduct")]
        public async Task<IActionResult> Put([FromForm] ProductDTO product)
        {
            var result = await _service.UpdateProduct(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("setproductstatus")]
        public async Task<IActionResult> PutStatus(ProductStatusDTO product)
        {
            var result = await _service.SetProductStatus(product);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPut("updateproductwithimg")]
        public async Task<IActionResult> PutWithImg(ProductWithImgDTO product)
        {
            var result = await _service.UpdateProductWithImg(product);
            return StatusCode(result.StatusCode,result);    
        }

        [HttpPut("updateproductnoneimg")]
        public async Task<IActionResult> PutNoneImg(ProductNoneImgDTO product)
        {
            var result = await _service.UpdateProductNoneImg(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("updateproductquantity")]
        public async Task<IActionResult> PutQuantity(ProductSubtractQuantityDTO product)
        {
            var result = await _service.UpdateStockQuantity(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("deleteproduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteProduct(id);
            return StatusCode(result.StatusCode, result);
        }



    }
}
