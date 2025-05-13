using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEaseProject.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public DateTime EventDate { get; set; } // Changed to DateTime to resolve CS05029

        [Required]
        [StringLength(500)]
        public required string EventDescription { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // Initialized to avoid CS8618

        public int BookingCount => Bookings?.Count ?? 0; // Added for views
    }
}