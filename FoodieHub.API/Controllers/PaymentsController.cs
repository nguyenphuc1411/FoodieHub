using FoodieHub.API.Models.DTOs;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    // API quản lý thanh toán
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]PaymentDTO payment)
        {
            bool result = await _service.Create(payment);
            return result ? Ok() : BadRequest();
        }
    }
}
