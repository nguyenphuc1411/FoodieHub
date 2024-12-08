using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Models.DTOs.User
{
    public class UserDTO
    {
        public string Id { get; set; } = default!;
        public string Fullname { get; set; } = default!;

        [Column(TypeName = "nvarchar(255)")]
        public string? Bio { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;
        public bool NotificationSubscription { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public string Email { get; set; } = default!;
        public string Role {  get; set; } = default!;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
