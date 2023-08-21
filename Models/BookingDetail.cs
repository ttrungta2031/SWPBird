using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class BookingDetail
    {
        public int Id { get; set; }
        public int? Amount { get; set; }
        public int? ServiceId { get; set; }
        public int? BookingId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Service Service { get; set; }
    }
}
