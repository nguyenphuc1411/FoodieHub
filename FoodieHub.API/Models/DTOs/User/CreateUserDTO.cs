using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.User
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Fullname is required")]
        [MinLength(4, ErrorMessage = "Fullname must be at least 4 characters long")]
        public string Fullname { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Bio must not exceed 255 characters")]
        public string? Bio { get; set; }
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "IsActive status is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}$", ErrorMessage = "Password must be at least 6 characters long, with at least one uppercase letter, one lowercase letter, one digit, and one special character")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
