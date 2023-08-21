using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public DateTime? DateCreate { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? AccountId { get; set; }
        public int? BookingId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Booking Booking { get; set; }
    }
}
