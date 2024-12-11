using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Authentication
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [MinLength(6, ErrorMessage = "Full Name must be at least 6 characters long.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match.")]
        public string ConfirmPassword { get; set; } = default!;
    }

}
