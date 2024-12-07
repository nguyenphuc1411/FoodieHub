using FoodieHub.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data
{
    public class ApplicationUser:IdentityUser
    {
        [Column(TypeName = "nvarchar(255)")]
        public string Fullname { get; set; } = default!;

        [Column(TypeName = "nvarchar(255)")]
        public string? Bio { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Avatar { get; set; }

        public bool IsActive { get; set; } = true;

        public bool NotificationSubscription { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.Now;

        // Foreign Key Collections
        public ICollection<Article> Articles { get; set; } = default!;
        public ICollection<Comment> Comments { get; set; } = default!;
        public ICollection<Favorite> Favorites { get; set; } = default!;
        public ICollection<Recipe> Recipes { get; set; } = default!;
        public ICollection<Rating> Ratings { get; set; } = default!;
        public ICollection<Order> Orders { get; set; } = default!;
        public ICollection<Review> Reviews { get; set; } = default!;

        public ICollection<Token> Token { get; set; } = default!;
        public ICollection<OrderCancellation> OrderCancellations { get; set; } = default!;
    }
}

