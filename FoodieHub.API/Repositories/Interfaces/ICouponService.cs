using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Coupon;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon?> Create(CouponDTO enntity);
        Task<ServiceResponse> GetByCode(string couponCode);
        Task<IEnumerable<GetCoupon>> Get();
        Task<IEnumerable<GetCoupon>> GetForUser();
        Task<GetCoupon?> GetDetail(int couponID);
        Task<bool> Update(int couponID,CouponDTO coupon);
        Task<bool> Delete(int couponID);
    }
}
