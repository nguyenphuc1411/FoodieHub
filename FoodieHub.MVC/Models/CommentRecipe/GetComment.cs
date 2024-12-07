namespace FoodieHub.MVC.Models.CommentRecipe
{
    public class GetComment
    {
        public int CommentID { get; set; } // Đảm bảo rằng thuộc tính này tồn tại
        public string CommentContent { get; set; }
        public DateTime CommentAt { get; set; }
        public string Avatar { get; set; }
        public string FullNameComment { get; set; }
        public int? ParentCommentID { get; set; }
        public string UserID { get; set; } // Thêm thuộc tính này
    }
}
