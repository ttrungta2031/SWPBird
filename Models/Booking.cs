using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class Booking
    {
        public Booking()
        {
            BookingDetails = new HashSet<BookingDetail>();
            BookingReports = new HashSet<BookingReport>();
            Feedbacks = new HashSet<Feedback>();
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public DateTime? DateBooking { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? BirdShelterId { get; set; }
        public int? AccountId { get; set; }
        public int? BirdProfileId { get; set; }
        public DateTime? DateFinish { get; set; }
        public double? Total { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }

        public virtual Account Account { get; set; }
        public virtual BirdProfile BirdProfile { get; set; }
        public virtual BirdShelter BirdShelter { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual ICollection<BookingReport> BookingReports { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
