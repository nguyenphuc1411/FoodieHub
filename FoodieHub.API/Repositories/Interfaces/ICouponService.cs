using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Coupon;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface ICouponService
    {
        Task<Coupon?> Create(Coupon enntity);
        Task<ServiceResponse> GetByCode(string couponCode);
        Task<ServiceResponse> Get();
        Task<ServiceResponse> GetDetail(int couponID);
        Task<bool> Update(int couponID,Coupon coupon);
        Task<bool> Delete(int couponID);
    }
}
