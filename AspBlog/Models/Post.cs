using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AspBlog.Helpers;

namespace AspBlog.Models
{

    public class Post
    {
        [Key]
        public int Id { get; set; }

        public string User_id { get; set; }
    
        [Required]
        [MinLength(3)]
        public string Title { get; set; }
        
        [Required]
        [MinLength(3)]
        public string Text { get; set; }

        public string Slug { get; set; }

        public DateTime Created_at { get; set; }

        [NotMapped]
        public string PrettyDate { get { return Created_at.ToString("dd. MMMM yyyy HH:mm"); } }

        [NotMapped]
        public string Teaser { get { return StringHelper.WordLimiter(Text,400); } }
    }
}