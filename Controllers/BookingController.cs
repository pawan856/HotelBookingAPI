using HotelBookingAPI.DTOs.AvailabilityDTOs;
using HotelBookingAPI.DTOs.BookingDTOs;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repository;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly BookingRepository _bookingRepository;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            BookingRepository bookingRepository,
            IEmailService emailService,
            IPaymentService paymentService,
            ILogger<BookingController> logger)
        {
            _bookingRepository = bookingRepository;
            _emailService = emailService;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Search for available rooms based on date range, guest count, amenities, and price
        /// </summary>
        [HttpGet("availability")]
        [AllowAnonymous] // Allow guests to search without login
        public async Task<APIResponse<List<RoomAvailabilityResponseDTO>>> SearchAvailability(
            [FromQuery] RoomAvailabilityRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new APIResponse<List<RoomAvailabilityResponseDTO>>(
                        HttpStatusCode.BadRequest, "Invalid request parameters");
                }

                if (request.Start >= request.End)
                {
                    return new APIResponse<List<RoomAvailabilityResponseDTO>>(
                        HttpStatusCode.BadRequest, "End date must be after start date");
                }

                var availableRooms = await _bookingRepository.SearchAvailableRoomsAsync(request);

                return new APIResponse<List<RoomAvailabilityResponseDTO>>(
                    availableRooms, 
                    $"Found {availableRooms.Count} available rooms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching room availability");
                return new APIResponse<List<RoomAvailabilityResponseDTO>>(
                    HttpStatusCode.InternalServerError, 
                    "Error searching availability", 
                    ex.Message);
            }
        }

        /// <summary>
        /// Create a new booking (status: Pending until payment confirmed)
        /// </summary>
        [HttpPost("create")]
        public async Task<APIResponse<BookingResponseDTO>> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new APIResponse<BookingResponseDTO>(
                        HttpStatusCode.BadRequest, "Invalid booking data");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                {
                    return new APIResponse<BookingResponseDTO>(
                        HttpStatusCode.Unauthorized, "User not authenticated");
                }

                var result = await _bookingRepository.CreateBookingAsync(dto, userId, userEmail);

                if (!result.Success)
                {
                    return new APIResponse<BookingResponseDTO>(
                        HttpStatusCode.BadRequest, result.ErrorMessage);
                }

                var response = new BookingResponseDTO
                {
                    BookingId = result.BookingId ?? 0,
                    Status = "Pending",
                    TotalAmount = 0, // TODO: Get from booking
                    Message = result.ErrorMessage,
                    PaymentClientSecret = result.PaymentClientSecret
                };

                return new APIResponse<BookingResponseDTO>(response, "Booking created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return new APIResponse<BookingResponseDTO>(
                    HttpStatusCode.InternalServerError, 
                    "Error creating booking", 
                    ex.Message);
            }
        }

        /// <summary>
        /// Cancel a booking and process refund based on cancellation policy
        /// </summary>
        [HttpPost("{bookingId}/cancel")]
        public async Task<APIResponse<object>> CancelBooking(int bookingId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return new APIResponse<object>(
                        HttpStatusCode.Unauthorized, "User not authenticated");
                }

                var result = await _bookingRepository.CancelBookingAsync(bookingId, userId);

                if (!result.Success)
                {
                    return new APIResponse<object>(
                        HttpStatusCode.BadRequest, result.ErrorMessage);
                }

                // Send cancellation email
                if (!string.IsNullOrEmpty(userEmail))
                {
                    await _emailService.SendCancellationConfirmationAsync(
                        userEmail, bookingId, result.RefundAmount);
                }

                return new APIResponse<object>(
                    new
                    {
                        bookingId,
                        status = "Cancelled",
                        refundAmount = result.RefundAmount,
                        message = result.ErrorMessage
                    },
                    "Booking cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking");
                return new APIResponse<object>(
                    HttpStatusCode.InternalServerError, 
                    "Error cancelling booking", 
                    ex.Message);
            }
        }

        /// <summary>
        /// Webhook endpoint for Stripe payment confirmations
        /// </summary>
        [HttpPost("webhook/stripe")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var signature = Request.Headers["Stripe-Signature"].ToString();

                if (!_paymentService.VerifyWebhookSignature(json, signature))
                {
                    _logger.LogWarning("Invalid Stripe webhook signature");
                    return Unauthorized();
                }

                // TODO: Parse webhook event and handle accordingly
                // Example events: payment_intent.succeeded, payment_intent.payment_failed
                
                _logger.LogInformation("Stripe webhook received");
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500);
            }
        }
    }
}
