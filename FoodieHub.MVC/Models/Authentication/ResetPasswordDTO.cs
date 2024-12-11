using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Authentication
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Data is required.")]
        public string Data { get; set; } = default!;

        [Required(ErrorMessage = "New password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string NewPassword { get; set; } = default!;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Password and Confirm Password must match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
