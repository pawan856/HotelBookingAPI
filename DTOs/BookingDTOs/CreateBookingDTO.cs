using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.BookingDTOs
{
    public class CreateBookingDTO
    {
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        [Range(1, 10)]
        public int GuestCount { get; set; }
        
        public string? SpecialRequests { get; set; }
    }
}
