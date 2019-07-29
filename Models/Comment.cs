using System.ComponentModel.DataAnnotations;

namespace NewsData.Models
{
    public class Comment
    {
        public int CommentID { get; set; }

        
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        
        [Display(Name = "Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        
        [Display(Name = "Message")]
        [Required]
        public string Message { get; set; }

        [Required]
        public int? ArticleID { get; set; }

        public Article Article { get; set; }
    }
}