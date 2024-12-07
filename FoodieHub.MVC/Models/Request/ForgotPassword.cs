using System.ComponentModel.DataAnnotations;

namespace FoodieHub.MVC.Models.Request
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }
    }
}
