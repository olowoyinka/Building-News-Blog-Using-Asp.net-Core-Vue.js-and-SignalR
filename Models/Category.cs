using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsData.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public ICollection<Article> Article { get; set; }
    }
}
