using Microsoft.AspNetCore.Mvc;
using SisandAirlines.Application.DTOs;
using SisandAirlines.Application.Services;

namespace SisandAirlines.Api.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _service;

        public PaymentsController(PaymentService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
        {
            var result = await _service.CreatePaymentAsync(request);
            return CreatedAtAction(nameof(GetByBookingId), new { bookingId = result.BookingId }, result);
        }

        [HttpGet("by-booking/{bookingId}")]
        public async Task<IActionResult> GetByBookingId(int bookingId)
        {
            var result = await _service.GetByBookingIdAsync(bookingId);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var list = await _service.GetPaymentsByCustomerAsync(customerId);
            return Ok(list);
        }
    }
}
