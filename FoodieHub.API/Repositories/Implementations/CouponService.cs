
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Coupon;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class CouponService:ICouponService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CouponService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Coupon?> Create(CouponDTO entity)
        {
            bool isExistCouponCode = await _context.Coupons.AnyAsync(x=>x.CouponCode == entity.CouponCode);      
            if(isExistCouponCode) return null;
            var newEntity = _mapper.Map<Coupon>(entity);
            await _context.Coupons.AddAsync(newEntity);
            var result = await _context.SaveChangesAsync();
            return result>0? newEntity : null;
        }

        public async Task<bool> Delete(int couponID)
        {
            var coupon = await _context.Coupons.FindAsync(couponID);
            if(coupon==null) return false;
            _context.Coupons.Remove(coupon);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<IEnumerable<GetCoupon>> Get()
        {
            return await _context.Coupons.OrderByDescending(x=>x.CreatedAt)
                .ProjectTo<GetCoupon>(_mapper.ConfigurationProvider).ToListAsync();
        }
        public async Task<GetCoupon?> GetDetail(int couponID)
        {
            return await _context.Coupons.ProjectTo<GetCoupon>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x=>x.CouponID==couponID);
           /* if(coupon.IsUsed)
            {
                var order = await _context.Orders.Where(x=>x.CouponID==couponID)
                    .ProjectTo<GetOrder>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
                var data = new
                {
                    Coupon = _mapper.Map<GetCoupon>(coupon),
                    Order = order,
                };
            }*/
        }
        public async Task<ServiceResponse> GetByCode(string couponCode)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x=>x.CouponCode==couponCode);
            if (coupon == null) return new ServiceResponse
            {
                Success = false,
                Message = "No coupon found with given coupon code",
                StatusCode = 404
            };
            if (!coupon.IsActive)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "This coupon is not active",
                    StatusCode = 400
                };
            }
            if (coupon.IsUsed)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "This coupon has already been used",
                    StatusCode = 400
                };
            }
            if (coupon.StartDate > DateTime.Now)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "This coupon has not started yet",
                    StatusCode = 400
                };
            }
            if(coupon.EndDate < DateTime.Now)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "This coupon has expired",
                    StatusCode = 400
                };
            }
            return new ServiceResponse
            {
                Success = true,
                Message = "Coupon applied successfully",
                Data = coupon,
                StatusCode = 200
            };
        }

        public async Task<bool> Update(int couponID,CouponDTO coupon)
        {
            var existCoupon = await _context.Coupons.FindAsync(couponID);
            if (existCoupon == null) return false;
            existCoupon.DiscountValue= coupon.DiscountValue;
            existCoupon.DiscountType= coupon.DiscountType;
            existCoupon.StartDate=coupon.StartDate;
            existCoupon.EndDate=coupon.EndDate;
            existCoupon.MinimumOrderAmount=coupon.MinimumOrderAmount;
            coupon.IsActive =coupon.IsActive;
            coupon.Note = coupon.Note;
            coupon.CouponCode = coupon.CouponCode;
            _context.Coupons.Update(existCoupon);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<GetCoupon>> GetForUser()
        {
            return await _context.Coupons
                .Where(x => x.IsActive && !x.IsUsed && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now)
                .OrderByDescending(x => x.CreatedAt)
               .ProjectTo<GetCoupon>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
