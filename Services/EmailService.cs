namespace HotelBookingAPI.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string toEmail, int bookingId, DateTime startDate, DateTime endDate);
        Task SendCancellationConfirmationAsync(string toEmail, int bookingId, decimal refundAmount);
        Task SendPaymentConfirmationAsync(string toEmail, int bookingId, decimal amount);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, int bookingId, DateTime startDate, DateTime endDate)
        {
            // TODO: Implement with MailKit
            _logger.LogInformation($"Sending booking confirmation email to {toEmail} for booking {bookingId}");
            await Task.CompletedTask;
        }

        public async Task SendCancellationConfirmationAsync(string toEmail, int bookingId, decimal refundAmount)
        {
            // TODO: Implement with MailKit
            _logger.LogInformation($"Sending cancellation confirmation email to {toEmail} for booking {bookingId}, refund: ${refundAmount}");
            await Task.CompletedTask;
        }

        public async Task SendPaymentConfirmationAsync(string toEmail, int bookingId, decimal amount)
        {
            // TODO: Implement with MailKit
            _logger.LogInformation($"Sending payment confirmation email to {toEmail} for booking {bookingId}, amount: ${amount}");
            await Task.CompletedTask;
        }
    }
}
