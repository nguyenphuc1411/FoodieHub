using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Authentication
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = default!;
    }
}
