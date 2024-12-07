using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Models.DTOs.User
{
    public class UserDTO
    {
        public string Id {  get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Avatar { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public string Role {  get; set; }
        public bool IsActive {  get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
