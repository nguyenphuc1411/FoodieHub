using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    // API quản lý tất cả đơn hàng bao gồm đơn hàng, đơn hàng chi tiết
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService service)
        {
            _orderService = service;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(OrderDTO order)
        {        
            var result = await _orderService.Create(order);
            return StatusCode(result.StatusCode, result);         
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet]
        public async Task<IActionResult> Get(int? pageSize,int? currentPage, DateOnly? orderDate,string? orderKey,bool? isDesc, string? status)
        {  
            var result = await _orderService.Get(pageSize,currentPage,orderDate,orderKey,isDesc, status);
            return StatusCode(result.StatusCode, result);         
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("recently")]
        public async Task<IActionResult> GetRecently()
        {
            var result = await _orderService.GetRecently();
            return Ok(result);
        }
        [Authorize]
        [HttpPatch("{orderID}")]
        public async Task<IActionResult> ChangeStatus(int orderID, string status)
        {
            var result = await _orderService.ChangeStatus(orderID,status);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize]
        [HttpPatch("ChangeStatusUser/{orderID}")]
        public async Task<IActionResult> ChangeStatusUser(int orderID, string status, string? cancellationReason = null)
        {
            var result = await _orderService.ChangeStatusUser(orderID, status, cancellationReason);
            return StatusCode(result.StatusCode, result);
        }
        // Lấy đơn hàng hiện tại của người dùng
        [Authorize]
        [HttpGet("ordered")]
        public async Task<IActionResult> GetByUser(int? pageSize, int? currentPage)
        {        
            var result = await _orderService.GetByUser(pageSize, currentPage);
            return StatusCode(result.StatusCode, result);         
        }
        [HttpGet("{orderID}")]
        public async Task<IActionResult> GetDetail(int orderID)
        {
            var result = await _orderService.GetDetail(orderID);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("OrderByUserId")]
        public async Task<IActionResult> GetOrderByUserId()
        {
            var result = await _orderService.GetOrderWithUserId();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("OrderDetailsByProductId")]
        public async Task<IActionResult> GetOrderDetailsByProductId()
        {
            var result = await _orderService.GetOrderDetailsWithProductId();
            return StatusCode(result.StatusCode, result);
        }

    }
}
