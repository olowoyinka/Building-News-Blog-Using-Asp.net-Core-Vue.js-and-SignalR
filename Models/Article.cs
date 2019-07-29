using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsData.Models
{
    public class Article
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Tittle")]
        [StringLength(55, MinimumLength = 10)]
        public string Tittle { get; set; }

        //[Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string ImageName { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public Category Category { get; set; }
    }
}