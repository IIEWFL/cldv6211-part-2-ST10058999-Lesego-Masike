using System.Collections.Generic;

namespace EventEaseProject.Models
{
    public class SearchViewModel
    {
        public List<Venue> Venues { get; set; } = new List<Venue>(); // Initialized to avoid CS8618
        public List<Event> Events { get; set; } = new List<Event>(); // Initialized to avoid CS8618
        public List<Booking> Bookings { get; set; } = new List<Booking>(); // Initialized to avoid CS8618
    }
}