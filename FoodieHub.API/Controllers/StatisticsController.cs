using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }
        [HttpGet("order")]
        public async Task<IActionResult> GetOrder([FromQuery]string by)
        {
            var data = await _service.GetOrder(by);
            if(data == null) return NotFound();
            return Ok(data);
        }
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] string by)
        {
            var data = await _service.GetRevenue(by);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpGet("customer")]
        public async Task<IActionResult> GetCustomer([FromQuery] string by)
        {
            var data = await _service.GetCustomer(by);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet("topselling")]
        public async Task<IActionResult> TopSelling([FromQuery] string by)
        {
            var data = await _service.TopSelling(by);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet("toprevenue")]
        public async Task<IActionResult> TopRevenue([FromQuery] string by)
        {
            var data = await _service.TopRevenue(by);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpGet("orderreports")]
        public async Task<IActionResult> Get([FromQuery] string by)
        {
            var data = await _service.OrderReports(by);
            return Ok(data);
        }
        [HttpGet("revenuereports")]
        public async Task<IActionResult> GetReport([FromQuery] string by)
        {
            var data = await _service.RevenueReports(by);
            return Ok(data);
        }

        
    }
}
