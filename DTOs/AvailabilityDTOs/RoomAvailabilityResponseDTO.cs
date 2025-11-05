namespace HotelBookingAPI.DTOs.AvailabilityDTOs
{
    public class RoomAvailabilityResponseDTO
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public int MaxOccupancy { get; set; }
        public decimal PricePerNight { get; set; }
        public List<string> Amenities { get; set; } = new();
        public string BedType { get; set; } = string.Empty;
        public string ViewType { get; set; } = string.Empty;
    }
}
