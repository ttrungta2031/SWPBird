using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class Account
    {
        public Account()
        {
            BirdProfiles = new HashSet<BirdProfile>();
            BirdShelters = new HashSet<BirdShelter>();
            Bookings = new HashSet<Booking>();
            Feedbacks = new HashSet<Feedback>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime? DateCreate { get; set; }
        public string Status { get; set; }

        public virtual ICollection<BirdProfile> BirdProfiles { get; set; }
        public virtual ICollection<BirdShelter> BirdShelters { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
