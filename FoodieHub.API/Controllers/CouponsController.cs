using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Coupon;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Azure.Core.HttpHeader;

namespace FoodieHub.API.Controllers
{
    // API quản lý mã giảm giá
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _service;

        public CouponsController(ICouponService service)
        {
            _service = service;
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPost]
        public async Task<ActionResult<Coupon>> Create(Coupon coupon)
        {
            var result = await _service.Create(coupon);
            if (result == null) return BadRequest();
            return Ok(result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPut("{couponID}")]
        public async Task<ActionResult> Update(int couponID,[FromBody]Coupon coupon)
        {
            if(couponID!=coupon.CouponID) return BadRequest();
            bool result = await _service.Update(couponID, coupon);
            return result ? NoContent() : BadRequest();
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpDelete("{couponID}")]
        public async Task<ActionResult> Delete(int couponID)
        {
            bool result = await _service.Delete(couponID);
            return result ? NoContent() : BadRequest();
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet("{couponID}")]
        public async Task<IActionResult> GetDetail(int couponID)
        {
            var result = await _service.GetDetail(couponID);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.Get();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("couponcode/{couponCode}")]
        public async Task<IActionResult> GetForUserUse(string couponCode)
        {
            var result = await _service.GetByCode(couponCode);
            return StatusCode(result.StatusCode, result);
        }
    }
}
