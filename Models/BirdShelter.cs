using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class BirdShelter
    {
        public BirdShelter()
        {
            Bookings = new HashSet<Booking>();
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public double? Rating { get; set; }
        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
