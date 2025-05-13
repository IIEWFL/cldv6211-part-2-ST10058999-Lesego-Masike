using System;
using System.ComponentModel.DataAnnotations;

namespace EventEaseProject.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event? Event { get; set; } // Nullable to avoid CS8618

        [Required]
        public int VenueId { get; set; }
        public Venue? Venue { get; set; } // Nullable to avoid CS8618

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
    }
}