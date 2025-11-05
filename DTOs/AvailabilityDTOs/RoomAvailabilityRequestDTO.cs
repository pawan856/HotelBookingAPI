using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.AvailabilityDTOs
{
    public class RoomAvailabilityRequestDTO
    {
        [Required]
        public DateTime Start { get; set; }
        
        [Required]
        public DateTime End { get; set; }
        
        public int? Guests { get; set; }
        
        public string? Amenities { get; set; } // Comma-separated list
        
        public decimal? MinPrice { get; set; }
        
        public decimal? MaxPrice { get; set; }
    }
}
