using SisandAirlines.Application.DTOs;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;
using SisandAirlines.Infrastructure.Repositories;

namespace SisandAirlines.Application.Services
{
    public class PaymentService
    {
        private readonly IUnitOfWork _uow;

        public PaymentService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentRequest request)
        {
            
            var booking = await _uow.Bookings.GetByIdAsync(request.BookingId);
            if (booking == null)
                throw new InvalidOperationException("Reserva não encontrada.");

            
            var payment = new Payment
            {
                BookingId = request.BookingId,
                Amount = request.Amount,
                Method = request.Method,      
                PaidAt = DateTime.UtcNow,
                ConfirmationCode = Guid.NewGuid().ToString("N").ToUpper()
            };

            await _uow.Payments.AddAsync(payment);

            
            try
            {
                booking.PaymentStatus = "PAID"; 
                await _uow.Bookings.UpdateAsync(booking);
            }
            catch
            {
                
            }

            
            var audit = new AuditLogRepository(_uow.Connection, _uow.Transaction);
            await audit.AddAsync("payment", "INSERT", payment.Id, null);

           
            await _uow.CommitAsync();

            
            return new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Method = payment.Method,
                PaidAt = payment.PaidAt,
                ConfirmationCode = payment.ConfirmationCode
            };
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByCustomerAsync(int customerId)
        {
            var payments = await _uow.Payments.GetByCustomerAsync(customerId);
            return payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                BookingId = p.BookingId,
                Amount = p.Amount,
                Method = p.Method,
                PaidAt = p.PaidAt,
                ConfirmationCode = p.ConfirmationCode
            });
        }

        public async Task<PaymentDto?> GetByBookingIdAsync(int bookingId)
        {
            var payment = await _uow.Payments.GetByBookingIdAsync(bookingId);
            if (payment == null) return null;

            return new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Method = payment.Method,
                PaidAt = payment.PaidAt,
                ConfirmationCode = payment.ConfirmationCode
            };
        }
    }
}
