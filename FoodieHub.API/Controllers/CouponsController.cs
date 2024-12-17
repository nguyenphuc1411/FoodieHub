using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Coupon;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Coupon>> Create(CouponDTO coupon)
        {
            var result = await _service.Create(coupon);
            if (result == null) return BadRequest();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{couponID}")]
        public async Task<ActionResult> Update(int couponID,[FromBody]CouponDTO coupon)
        {
            bool result = await _service.Update(couponID, coupon);
            return result ? NoContent() : BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{couponID}")]
        public async Task<ActionResult> Delete(int couponID)
        {
            bool result = await _service.Delete(couponID);
            return result ? NoContent() : BadRequest();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{couponID}")]
        public async Task<ActionResult<GetCoupon>> GetDetail(int couponID)
        {
            var result = await _service.GetDetail(couponID);
            if(result == null) return NotFound();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCoupon>>> Get()
        {
            var result = await _service.Get();
            return Ok(result);
        }
        [HttpGet("couponcode/{couponCode}")]
        public async Task<IActionResult> GetForUserUse(string couponCode)
        {
            var result = await _service.GetByCode(couponCode);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<GetCoupon>>> GetForUser()
        {
            var result = await _service.GetForUser();
            return Ok(result);
        }
    }
}
