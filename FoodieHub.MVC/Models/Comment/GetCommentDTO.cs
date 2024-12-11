namespace FoodieHub.MVC.Models.Comment
{
    public class GetCommentDTO
    {
        public int CommentID { get; set; }
        public int? RecipeID { get; set; }
        public int? ArticleID { get; set; }
        public string CommentContent { get; set; } = default!;
        public DateTime CommentedAt { get; set; }
        public string UserID { get; set; } = default!;
        public string Avatar { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }
}
