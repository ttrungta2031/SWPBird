using System;
using System.Collections.Generic;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class Service
    {
        public Service()
        {
            BookingDetails = new HashSet<BookingDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public double? Price { get; set; }
        public int? Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? DateChange { get; set; }
        public int? BirdShelterId { get; set; }

        public virtual BirdShelter BirdShelter { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
