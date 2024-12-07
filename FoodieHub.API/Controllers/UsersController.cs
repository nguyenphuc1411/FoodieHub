using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    // API quản lý người dùng thêm sửa xóa phân quyền ...
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IQRCodeService _qRCodeService;
        public UsersController(IUserService service, IQRCodeService qRCodeService)
        {
            _service = service;
            _qRCodeService = qRCodeService;
        }      
        [HttpGet]
        public async Task<IActionResult> Get(string? role,string? email,int pageSize,int currentPage)
        {
            var result = await _service.Get(role,email,pageSize,currentPage);
            return StatusCode(result.StatusCode, result);
        }
        /*[Authorize(Policy ="RequireAdmin")]*/
        [HttpPost]
        public async Task<IActionResult> Create(CreateUser createUser)
        {
            var result = await _service.Create(createUser);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPatch("disable/{id}")]
        public async Task<IActionResult> Disble(string id)
        {
            var result = await _service.Disable(id);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(string id)
        {
            var result = await _service.Restore(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(string id)
        {
            var result = await _service.GetByID(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("/api/qrcode")]
        public IActionResult GetQRCode()
        {
            byte[] data = _qRCodeService.GenerateQRCode("Text");
            return Ok(Convert.ToBase64String(data));
        }

        
    }
}
