using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Category;
using FoodieHub.API.Models.DTOs.Comment;
using FoodieHub.API.Models.DTOs.Contact;
using FoodieHub.API.Models.DTOs.Coupon;
using FoodieHub.API.Models.DTOs.Order;
using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.DTOs.Review;
using FoodieHub.API.Models.DTOs.User;

namespace FoodieHub.API.Configurations.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDTO>();

            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<RecipeCategoryDTO, RecipeCategory>().ReverseMap();
            CreateMap<ArticleCategoryDTO, ArticleCategory>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
            CreateMap<ProductImageDTO, ProductImage>().ReverseMap();
            CreateMap<ReviewDTO, Review>().ReverseMap();
            CreateMap<RecipeCategoryDTO, RecipeCategory>().ReverseMap();
            CreateMap<ArticleCategoryDTO, ArticleCategory>().ReverseMap();

            CreateMap<ContactDTO, Contact>().ReverseMap();
            CreateMap<Contact, GetContact>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))  // Lấy tên sản phẩm
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.MainImage)); // Lấy hình ảnh sản phẩm

            CreateMap<CouponDTO, Coupon>();
            CreateMap<Coupon, GetCoupon>();
            CreateMap<Coupon, CouponForOrder>();

            CreateMap<OrderDTO, Order>()
            .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());
            CreateMap<Order, GetOrder>()
               .ForMember(x => x.Fullname, opt => opt.MapFrom(src => src.User.Fullname));

            CreateMap<OrderDetail, ProductForOrder>()
                 .ForMember(x => x.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                  .ForMember(x => x.ProductImage, opt => opt.MapFrom(src => src.Product.MainImage));
            CreateMap<Payment, PaymentDTO>();
            CreateMap<OrderCancellation, OrderCancel>();
            CreateMap<OrderStatusHistory, OrderStatus>();

            CreateMap<Order, GetDetailOrder>()
                .ForMember(x => x.Fullname, opt => opt.MapFrom(src => src.User.Fullname))
                .ForMember(x => x.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(x => x.ProductForOrder, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(x => x.Coupon, opt => opt.MapFrom(src => src.Coupon))
                .ForMember(x => x.Payment, opt => opt.MapFrom(src => src.Payment))
                .ForMember(x => x.OrderStatues, opt => opt.MapFrom(src => src.OrderStatusHistories))
                .ForMember(x => x.OrderCancel, opt => opt.MapFrom(src => src.OrderCancellation));


            CreateMap<Order, RecentlyOrder>()
           .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount-(src.Discount??0)-(src.DiscountOfCoupon??0)))
          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src=>src.User.Fullname));

            CreateMap<OrderDetail, DetailDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.MainImage));


            // Article
            CreateMap<Article, GetArticleDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Fullname))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.ArticleCategory.CategoryName))
                .ForMember(dest => dest.TotalComments, opt => opt.MapFrom(src => src.ArticleComments.Count()))
                .ForMember(dest => dest.TotalFavorites, opt => opt.MapFrom(src => src.FavoriteArticles.Count()));
            CreateMap<UpdateArticleDTO, Article>();


       
            // Recipes
            CreateMap<CreateRecipeDTO, Recipe>();

            CreateMap<Recipe, GetRecipeDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Fullname))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.RecipeCategory.CategoryName))
                .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Ratings.Any()? src.Ratings.Count():0))
                .ForMember(dest => dest.RatingAverage, opt => opt.MapFrom(src => src.Ratings.Any()? src.Ratings.Average(x => x.RatingValue):0))
                .ForMember(dest => dest.TotalComments, opt => opt.MapFrom(src => src.Comments.Any()? src.Comments.Count():0))
                .ForMember(dest => dest.TotalFavorites, opt => opt.MapFrom(src => src.Favorites.Any()? src.Favorites.Count():0));


            // Detail
            CreateMap<Ingredient, IngredientVM>();
            CreateMap<RecipeStep, RecipeStepVM>();
            CreateMap<Recipe, DetailRecipeDTO>()
                           .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Fullname))
                           .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                           .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.RecipeCategory.CategoryName))
                           .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Ratings.Any() ? src.Ratings.Count(): 0))
                           .ForMember(dest => dest.RatingAverage, opt => opt.MapFrom(src => src.Ratings.Any() ? src.Ratings.Average(x => x.RatingValue) : 0))
                           .ForMember(dest => dest.TotalComments, opt => opt.MapFrom(src => src.Comments.Any() ? src.Comments.Count() : 0))
                           .ForMember(dest => dest.TotalFavorites, opt => opt.MapFrom(src => src.Favorites.Any() ? src.Favorites.Count() : 0))
                           .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
                           .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.RecipeSteps))
                           .ForMember(dest => dest.RelativeProducts, opt => opt.MapFrom(src => src.RecipeProducts.Select(x => x.ProductID)));

                        
            // Comment
            CreateMap<Comment, GetCommentDTO>()
                .ForMember(x => x.FullName, opt => opt.MapFrom(src => src.User.Fullname))
                .ForMember(x => x.Avatar, opt => opt.MapFrom(src => src.User.Avatar));
            CreateMap<CommentDTO, Comment>();
        }
    }
}
