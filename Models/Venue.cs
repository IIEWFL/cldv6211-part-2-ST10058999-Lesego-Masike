using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEaseProject.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        [Required]
        [StringLength(250)]
        public required string VenueName { get; set; }

        [Required]
        [StringLength(250)]
        public required string VenueLocation { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int VenueCapacity { get; set; }

        [Required]
        public required string VenueImage { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>(); // Initialized to avoid CS8618

        public string ImageUrl => VenueImage; // Added for views
    }
}