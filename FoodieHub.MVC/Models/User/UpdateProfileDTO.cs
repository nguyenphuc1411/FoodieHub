using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.User
{
    public class UpdateProfileDTO
    {
        [Required(ErrorMessage = "Fullname is required")]
        [MinLength(4, ErrorMessage = "Fullname must be at least 4 characters long")]
        public string Fullname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Avatar { get; set; }

        [MaxLength(255, ErrorMessage = "Bio must not exceed 255 characters")]
        public string? Bio { get; set; }

        public IFormFile? File { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$", ErrorMessage = "Old password is invalid")]
        public string? OldPassword { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$", ErrorMessage = "New password is invalid")]
        public string? NewPassword { get; set; }
    }
}
