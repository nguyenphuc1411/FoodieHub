using AutoMapper;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Data;
using FoodieHub.API.Models.DTOs.Review;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class ReviewService: IReviewService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public ReviewService(AppDbContext appDbContext, IMapper mapper, IAuthService authService)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<ServiceResponse> GetListReview(int id)
        {

            var obj = await _appDbContext.Reviews.Where(x => x.ProductID == id).Select(x => new 
            {
                x.ReviewID,
                x.RatingValue,
                x.ReviewContent,
                x.ReviewedAt,
                x.ProductID,
                x.User.UserName,
                x.User.Avatar,
                x.User.Id

                
            }).ToListAsync();
            return new ServiceResponse
            {
                Success = true,
                Message = "Get review successfully",
                Data = obj,
                StatusCode = 200
            };
        }

        public async Task<ServiceResponse> UpdateReview(ReviewDTO reviewDTO)
        {
            var review = await _appDbContext.Reviews.FindAsync(reviewDTO.ReviewID);
            var userId = _authService.GetUserID();

            if (review == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Review not found.",
                    StatusCode = 404
                };
            }
            if (review.UserID != userId)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Unauthorized to update this review.",
                    StatusCode = 403
                };
            }


            
            review.RatingValue = reviewDTO.RatingValue;
            review.ReviewContent = reviewDTO.ReviewContent;
            review.ReviewedAt = DateTime.UtcNow;

            _appDbContext.Reviews.Update(review);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Review updated successfully.",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to update review.",
                    StatusCode = 400
                };
            }
        }


        public async Task<ServiceResponse> AddNewReview(ReviewDTO reviewDTO)
        {
            var product = await _appDbContext.Products.FindAsync(reviewDTO.ProductID);

            var userId = _authService.GetUserID();

            if (product == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Product not found.",
                    StatusCode = 404
                };
            }

            
            var obj = new ReviewDTO
            {
                ProductID = reviewDTO.ProductID,
                UserID = userId,
                RatingValue = reviewDTO.RatingValue,
                ReviewContent = reviewDTO.ReviewContent,
                ReviewedAt = reviewDTO.ReviewedAt,
            };

            var review = _mapper.Map<Review>(obj);
            await _appDbContext.Reviews.AddAsync(review);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Review added successfully.",
                    StatusCode = 201,
                    Data = result
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to add review.",
                    StatusCode = 400
                };
            }
        }

        public async Task<ServiceResponse> DeleteNewReview(int id)
        {
            var review = await _appDbContext.Reviews.FindAsync(id);
            if (review == null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Review not found.",
                    StatusCode = 404
                };
            }

            _appDbContext.Reviews.Remove(review);
            var result = await _appDbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Review deleted successfully.",
                    StatusCode = 200
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to delete review.",
                    StatusCode = 400
                };
            }
        }
    }
}
