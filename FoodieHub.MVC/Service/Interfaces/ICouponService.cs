using FoodieHub.MVC.Models.Coupon;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface ICouponService
    {
        Task<bool> Create(CouponDTO enntity);
        Task<IEnumerable<GetCoupon>?> Get();
        Task<IEnumerable<GetCoupon>> GetForUser();
        Task<GetCoupon?> GetDetail(int couponID);
        Task<bool> Update(int couponID, CouponDTO coupon);
        Task<bool> Delete(int couponID);
    }
}
