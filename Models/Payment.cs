namespace HotelBookingAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Provider { get; set; } = "Stripe"; // Stripe, PayPal, etc.
        public string? ProviderPaymentId { get; set; } // Stripe PaymentIntent ID
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Pending"; // Pending, Succeeded, Failed, Refunded
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
