using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class BirdProfile
    {
        public BirdProfile()
        {
            Bookings = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
