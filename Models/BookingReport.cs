using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class BookingReport
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public string MsgHost { get; set; }
        public string MsgCustomer { get; set; }
        public string Status { get; set; }
        public int? BookingId { get; set; }

        public virtual Booking Booking { get; set; }
    }
}
