using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
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
        public async Task<IActionResult> Create([FromBody] OrderDTO order)
        {
            var result = await _orderService.Create(order);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<PaginatedModel<GetOrder>>> Get([FromQuery] QueryOrderModel queryOrder)
        {
            var result = await _orderService.Get(queryOrder);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("ordered")]
        public async Task<ActionResult<PaginatedModel<GetOrder>>> GetByUser([FromQuery] QueryOrderModel queryOrder)
        {
            var result = await _orderService.GetByUser(queryOrder);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderID}")]
        public async Task<IActionResult> ChangeStatus(int orderID, [FromQuery] string status)
        {
            var result = await _orderService.ChangeStatus(orderID, status);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize]
        [HttpPatch("ChangeStatusUser/{orderID}")]
        public async Task<IActionResult> ChangeStatusUser(int orderID, [FromQuery] string status, [FromQuery] string? cancellationReason)
        {
            var result = await _orderService.ChangeStatusUser(orderID, status, cancellationReason);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByID([FromRoute]int id)
        {
            var result = await _orderService.GetByID(id);
            if (result == null) return NotFound();
            return Ok(result);
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
