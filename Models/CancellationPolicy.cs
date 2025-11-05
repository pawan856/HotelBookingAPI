namespace HotelBookingAPI.Models
{
    public class CancellationPolicy
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CancellationWindowHours { get; set; } // Hours before check-in
        public decimal RefundPercentage { get; set; } // 0-100
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
