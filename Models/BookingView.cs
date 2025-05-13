using System;

namespace EventEaseProject.Models
{
    public class BookingView
    {
        public int BookingID { get; set; }
        public string VenueName { get; set; } = string.Empty; // Initialized to avoid CS8618
        public string VenueLocation { get; set; } = string.Empty; // Initialized to avoid CS8618
        public int VenueCapacity { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; } = string.Empty; // Initialized to avoid CS8618
        public DateTime BookingDate { get; set; }
    }
}