using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.AvailabilityDTOs;
using HotelBookingAPI.DTOs.BookingDTOs;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Repository
{
    public class BookingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(ApplicationDbContext context, IPaymentService paymentService, ILogger<BookingRepository> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<List<RoomAvailabilityResponseDTO>> SearchAvailableRoomsAsync(RoomAvailabilityRequestDTO request)
        {
            // Get all confirmed bookings that overlap with the requested dates
            var overlappingBookings = await _context.Bookings
                .Where(b => b.Status == "Confirmed" &&
                           b.StartDate < request.End &&
                           b.EndDate > request.Start)
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            // TODO: Query rooms from your existing room database
            // This is a placeholder - integrate with your existing RoomRepository
            var availableRooms = new List<RoomAvailabilityResponseDTO>();
            
            _logger.LogInformation($"Found {availableRooms.Count} available rooms for dates {request.Start} to {request.End}");
            
            return availableRooms;
        }

        public async Task<(bool Success, int? BookingId, string? PaymentClientSecret, string ErrorMessage)> CreateBookingAsync(
            CreateBookingDTO dto, string userId, string userEmail)
        {
            // Check for overlapping bookings
            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.RoomId == dto.RoomId &&
                b.Status == "Confirmed" &&
                b.StartDate < dto.EndDate &&
                b.EndDate > dto.StartDate);

            if (hasOverlap)
            {
                return (false, null, null, "Room is not available for the selected dates");
            }

            // TODO: Get room price from your room database
            var pricePerNight = 100m; // Placeholder
            var nights = (dto.EndDate - dto.StartDate).Days;
            var totalAmount = pricePerNight * nights;

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                RoomId = dto.RoomId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                GuestCount = dto.GuestCount,
                TotalAmount = totalAmount,
                Status = "Pending",
                SpecialRequests = dto.SpecialRequests,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Create payment intent
            var paymentResult = await _paymentService.CreatePaymentIntentAsync(
                totalAmount, "USD", booking.Id);

            if (!paymentResult.Success)
            {
                return (false, booking.Id, null, paymentResult.ErrorMessage ?? "Payment creation failed");
            }

            // Create payment record
            var payment = new Payment
            {
                BookingId = booking.Id,
                Provider = "Stripe",
                ProviderPaymentId = paymentResult.PaymentIntentId,
                Amount = totalAmount,
                Currency = "USD",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return (true, booking.Id, paymentResult.ClientSecret, "Booking created successfully");
        }

        public async Task<(bool Success, decimal RefundAmount, string ErrorMessage)> CancelBookingAsync(int bookingId, string userId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            
            if (booking == null)
            {
                return (false, 0, "Booking not found");
            }

            if (booking.UserId != userId)
            {
                return (false, 0, "Unauthorized");
            }

            if (booking.Status == "Cancelled")
            {
                return (false, 0, "Booking already cancelled");
            }

            // Get cancellation policy
            var policy = await _context.CancellationPolicies
                .Where(p => p.IsActive)
                .OrderBy(p => p.CancellationWindowHours)
                .FirstOrDefaultAsync();

            var hoursUntilCheckIn = (booking.StartDate - DateTime.UtcNow).TotalHours;
            var refundPercentage = 0m;

            if (policy != null && hoursUntilCheckIn >= policy.CancellationWindowHours)
            {
                refundPercentage = policy.RefundPercentage / 100m;
            }

            var refundAmount = booking.TotalAmount * refundPercentage;

            // Update booking status
            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;

            // Process refund if there was a payment
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId && p.Status == "Succeeded");

            if (payment != null && refundAmount > 0)
            {
                var refundResult = await _paymentService.RefundPaymentAsync(
                    payment.ProviderPaymentId ?? "", refundAmount);

                if (refundResult.Success)
                {
                    payment.Status = "Refunded";
                    payment.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return (true, refundAmount, "Booking cancelled successfully");
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdatePaymentStatusAsync(string providerPaymentId, string status)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.ProviderPaymentId == providerPaymentId);

            if (payment == null) return false;

            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;

            // If payment succeeded, update booking to confirmed
            if (status == "Succeeded")
            {
                var booking = await _context.Bookings.FindAsync(payment.BookingId);
                if (booking != null)
                {
                    booking.Status = "Confirmed";
                    booking.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
