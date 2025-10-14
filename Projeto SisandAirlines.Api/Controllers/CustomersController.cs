using Microsoft.AspNetCore.Mvc;
using SisandAirlines.Application.DTOs;
using SisandAirlines.Application.Services;

namespace SisandAirlines.Api.Controllers
{
    [ApiController]
    [Route("customers")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _service;

        public CustomersController(CustomerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var customer = await _service.CreateCustomerAsync(request);
            return CreatedAtAction(nameof(GetAll), new { email = customer.Email }, customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }
    }
}
