namespace FoodieHub.API.Models.DTOs.Comment
{
    public class CommentDTO
    {
        public int CommentID { get; set; }
        public int? RecipeID { get; set; }
        public int? ArticleID { get; set; }
        public string CommentContent { get; set; } = default!;
        public DateTime CommentAt { get; set; }
        public string UserID { get; set; } = default!;
        public string Avatar { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
}
