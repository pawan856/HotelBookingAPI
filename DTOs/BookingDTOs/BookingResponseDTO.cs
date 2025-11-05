namespace HotelBookingAPI.DTOs.BookingDTOs
{
    public class BookingResponseDTO
    {
        public int BookingId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? PaymentClientSecret { get; set; } // For Stripe frontend integration
    }
}
