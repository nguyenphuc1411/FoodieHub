using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Data.Entities
{
    public class Article
    {
        [Key]
        public int ArticleID { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Title { get; set; } = default!;


        [Column(TypeName = "varchar(255)")]
        public string MainImage { get; set; }= default!;


        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foregin Key Property

        [Column(TypeName = "nvarchar(450)")]
        public string UserID { get; set; } = default!;

        public int CategoryID { get; set; }

        // Foreign Key Link
        public  ApplicationUser User { get; set; } = default!;

        public  ArticleCategory ArticleCategory { get; set; } = default!;

        // Foreign Key Collections
        public ICollection<Comment> ArticleComments { get; set; } = default!;
        public ICollection<Favorite> FavoriteArticles { get; set; } = default!;
    }
}
