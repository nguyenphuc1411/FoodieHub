using FoodieHub.MVC.Models.Coupon;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Service.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly HttpClient _httpClient;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<bool> Create(CouponDTO enntity)
        {
            var response = await _httpClient.PostAsJsonAsync("coupons", enntity);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int couponID)
        {
            var response = await _httpClient.DeleteAsync("coupons/"+couponID);
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<GetCoupon>?> Get()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetCoupon>>("coupons");
        }

        public async Task<GetCoupon?> GetDetail(int couponID)
        {
            return await _httpClient.GetFromJsonAsync<GetCoupon>($"coupons/{couponID}");
        }

        public async Task<IEnumerable<GetCoupon>> GetForUser()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GetCoupon>>("coupons/users")?? new List<GetCoupon>();
        }

        public async Task<bool> Update(int couponID, CouponDTO coupon)
        {
            var response = await _httpClient.PutAsJsonAsync($"coupons/{couponID}", coupon);
            return response.IsSuccessStatusCode;
        }
    }
}
