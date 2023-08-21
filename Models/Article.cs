using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class Article
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreate { get; set; }
        public string Status { get; set; }
    }
}
