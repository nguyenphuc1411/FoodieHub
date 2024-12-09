using FoodieHub.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }


        public required DbSet<ArticleCategory> ArticleCategories { get; set; }
        public required DbSet<RecipeCategory> RecipeCategories { get; set; }
        public required DbSet<Category> Categories { get; set; }

        public required DbSet<Article> Articles { get; set; }
        public required DbSet<Comment> Comments { get; set; }
        public required DbSet<Favorite> Favorites { get; set; }
        public required DbSet<Recipe> Recipes { get; set; }
        public required DbSet<Rating> Ratings { get; set; }
        public required DbSet<Product> Products { get; set; }
        public required DbSet<ProductImage> ProductImages { get; set; }
        public required DbSet<Review> Reviews { get; set; }

        public required DbSet<Coupon> Coupons { get; set; }
        public required DbSet<Payment> Payments { get; set; }

        public required DbSet<Order> Orders { get; set; }
        public required DbSet<OrderDetail> OrderDetails { get; set; }

        public required DbSet<Token> Tokens { get; set; }
        public required DbSet<Contact> Contacts { get; set; }
        public required DbSet<OrderCancellation> OrderCancellations { get; set; }
        public required DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public required DbSet<RecipeStep> RecipeSteps { get; set; }
        public required DbSet<Ingredient> Ingredients { get; set; }
        public required DbSet<RecipeProduct> RecipeProducts { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Rating>()
              .HasKey(r => new { r.RecipeID, r.UserID });
       
            builder.Entity<OrderDetail>()
              .HasKey(od => new { od.ProductID, od.OrderID });

            builder.Entity<RecipeProduct>()
                .HasKey(x => new { x.RecipeID, x.ProductID });
            // Cấu hình khóa ngoại

            // USERS
            builder.Entity<ApplicationUser>(options =>
            {
                options.HasMany(u => u.Articles)
               .WithOne(a => a.User).HasForeignKey(a => a.UserID);

                options.HasMany(u => u.Comments)
               .WithOne(ac => ac.User).HasForeignKey(ac => ac.UserID);

                options.HasMany(u => u.Favorites)
              .WithOne(fa => fa.User).HasForeignKey(fa => fa.UserID);

                options.HasMany(u => u.Orders)
             .WithOne(o => o.User).HasForeignKey(o => o.UserID);

                options.HasMany(u => u.Reviews)
             .WithOne(r => r.User).HasForeignKey(r => r.UserID);

                options.HasMany(u => u.Recipes)
             .WithOne(r => r.User).HasForeignKey(r => r.UserID);

                options.HasMany(u => u.Ratings)
             .WithOne(r => r.User).HasForeignKey(r => r.UserID);

                options.HasMany(u => u.Token)
                .WithOne(x => x.User).HasForeignKey(x => x.UserID);

                options.HasMany(u => u.OrderCancellations)
              .WithOne(x => x.User).HasForeignKey(x => x.UserID);

            });

            // ARTICLE CATEGORIES
            builder.Entity<ArticleCategory>().HasMany(ac => ac.Articles)
                .WithOne(a => a.ArticleCategory).HasForeignKey(a => a.CategoryID);

            // ARTICLE
            builder.Entity<Article>(options =>
            {
                options.HasMany(a => a.ArticleComments)
                .WithOne(ac => ac.Article).HasForeignKey(ac => ac.ArticleID);

                options.HasMany(a => a.FavoriteArticles)
               .WithOne(fa => fa.Article).HasForeignKey(fa => fa.ArticleID);
            });

            // RECIPE CATEGORIES
            builder.Entity<RecipeCategory>().HasMany(rc => rc.Recipes)
                .WithOne(r => r.RecipeCategory).HasForeignKey(r => r.CategoryID);

            // RECIPE
            builder.Entity<Recipe>(options =>
            {
                options.HasMany(r => r.Favorites)
                .WithOne(fr => fr.Recipe).HasForeignKey(fr => fr.RecipeID);

                options.HasMany(r => r.Ratings)
               .WithOne(r => r.Recipe).HasForeignKey(r => r.RecipeID);

                options.HasMany(r => r.Comments)
              .WithOne(rc => rc.Recipe).HasForeignKey(rc => rc.RecipeID);

                options.HasMany(r => r.Ingredients)
            .WithOne(rc => rc.Recipe).HasForeignKey(rc => rc.RecipeID);

                options.HasMany(r => r.RecipeSteps)
              .WithOne(rc => rc.Recipe).HasForeignKey(rc => rc.RecipeID);


                options.HasMany(r => r.RecipeProducts)
              .WithOne(rc => rc.Recipe).HasForeignKey(rc => rc.RecipeID);

            });

            //  CATEGORIES
            builder.Entity<Category>().HasMany(c => c.Products)
                .WithOne(p => p.Category).HasForeignKey(p => p.CategoryID);



            // PRODUCT
            builder.Entity<Product>(options =>
            {
                options.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product).HasForeignKey(pi => pi.ProductID);

                options.HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product).HasForeignKey(od => od.ProductID);

                options.HasMany(p => p.Reviews)
               .WithOne(r => r.Product).HasForeignKey(r => r.ProductID);

                options.HasMany(p => p.Contacts)
              .WithOne(r => r.Product).HasForeignKey(r => r.ProductID);

                options.HasMany(p => p.RecipeProducts)
             .WithOne(r => r.Product).HasForeignKey(r => r.ProductID);
            });

            // ORDER
            builder.Entity<Order>(options =>
            {
                options.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order).HasForeignKey(od => od.OrderID);

                options.HasMany(o => o.OrderStatusHistories)
                .WithOne(od => od.Order).HasForeignKey(od => od.OrderID);

                options.HasOne(o => o.Coupon).WithOne(c => c.Order)
                .HasForeignKey<Order>(o => o.CouponID);

                options.HasOne(o => o.Payment).WithOne(c => c.Order)
              .HasForeignKey<Payment>(p => p.OrderID) 
              .IsRequired(false);

                options.HasOne(o => o.OrderCancellation).WithOne(c => c.Order)
             .HasForeignKey<OrderCancellation>(o => o.OrderID);

            });

            // Điều chỉnh các khóa ngoại action delete là no action

            var listEntity = builder.Model.GetEntityTypes();
            foreach (var entity in listEntity)
            {
                foreach (var fk in entity.GetForeignKeys())
                {
                    if (fk.DeleteBehavior == DeleteBehavior.Cascade || fk.DeleteBehavior == DeleteBehavior.ClientCascade)
                    {
                        fk.DeleteBehavior = DeleteBehavior.NoAction;
                    }
                }
            }

            // Dữ liệu mặc định cho Role và User admin

            builder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // Khởi tạo người dùng mặc định
            var defaulAdmin = new ApplicationUser
            {
                Id = "b6d4f2fd-ae9a-475f-a186-264597704bae",
                Fullname = "NguyenThanhPhuc",
                Email = "nguyenphuc14112003@gmail.com",
                UserName = "nguyenphuc14112003@gmail.com",
                NormalizedUserName = "NGUYENPHUC14112003@GMAIL.COM",
                NormalizedEmail = "NGUYENPHUC14112003@GMAIL.COM",
                IsActive = true,
                JoinedAt = DateTime.Now
            };
            var defaulUser = new ApplicationUser
            {
                Id = "b6d4f2fd-ae9a-475f-a186-264597704bat",
                Fullname = "User Name",
                Email = "user@gmail.com",
                UserName = "user@gmail.com",
                NormalizedUserName = "USER@GMAIL.COM",
                NormalizedEmail = "USER@GMAIL.COM",
                IsActive = true,
                JoinedAt = DateTime.Now
            };
            var passwordAdminHash = new PasswordHasher<ApplicationUser>().HashPassword(defaulUser, "Admin@123");
            var passwordUserHash = new PasswordHasher<ApplicationUser>().HashPassword(defaulUser, "User@123");
            defaulAdmin.PasswordHash = passwordAdminHash;
            defaulUser.PasswordHash = passwordUserHash;

            builder.Entity<ApplicationUser>()
                .HasData(defaulUser, defaulAdmin);

            // Khởi tạo IdentityUserRole
            builder.Entity<IdentityUserRole<string>>()
                .HasData(
                    new IdentityUserRole<string>
                    {
                        UserId = defaulAdmin.Id,
                        RoleId = "1"
                    },
                    new IdentityUserRole<string>
                    {
                        UserId = defaulUser.Id,
                        RoleId = "2"
                    }
                );
        }
    }
}
