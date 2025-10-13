using SisandAirlines.Application.DTOs;
using SisandAirlines.Domain.Entities;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Application.Services
{
    public class BookingService
    {
        private readonly IUnitOfWork _uow;

        public BookingService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingRequest request)
        {
            Console.WriteLine($"🟢 BookingService.CreateBookingAsync iniciado: FlightInstanceId={request.FlightInstanceId}");

            Console.WriteLine($"_uow == null ? {_uow == null}");
            Console.WriteLine($"_uow.Bookings == null ? {_uow?.Bookings == null}");
            bool taken = await _uow.Bookings.SeatAlreadyBookedAsync(request.FlightInstanceId, request.SeatId);
            if (taken)
                throw new InvalidOperationException("This seat is already booked for this flight.");

            var booking = new Booking
            {
                CustomerId = request.CustomerId,
                FlightInstanceId = request.FlightInstanceId,
                SeatId = request.SeatId,
                FareId = request.FareId,
                PaymentStatus = "PENDING"
            };

            await _uow.Bookings.AddAsync(booking);
            _uow.CommitAsync();

            return new BookingDto
            {
                CustomerId = booking.CustomerId,
                FlightInstanceId = booking.FlightInstanceId,
                SeatId = booking.SeatId,
                FareId = booking.FareId,
                PaymentStatus = booking.PaymentStatus,
                BookingDate = booking.BookingDate
            };
        }

        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            var booking = await _uow.Bookings.GetByIdAsync(id);
            if (booking == null)
                return null;

            return new BookingDto
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                FlightInstanceId = booking.FlightInstanceId,
                SeatId = booking.SeatId,
                FareId = booking.FareId,
                PaymentStatus = booking.PaymentStatus,
                BookingDate = booking.BookingDate
            };
        }
    }
}
