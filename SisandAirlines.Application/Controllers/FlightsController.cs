using Microsoft.AspNetCore.Mvc;
using SisandAirlines.Application.Services;
using System.Runtime.InteropServices;

namespace SisandAirlines.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly FlightService _flightService;

        public FlightsController(FlightService flightService)
        {
            _flightService = flightService;
        }

        /// <summary>
        /// Retorna os voos disponíveis conforme a data informada (Curitiba → São Paulo)
        /// </summary>
        /// <param name="date">Data do voo (YYYY-MM-DD)</param>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableFlights([FromQuery] DateTime date)
        {
            if (date == default)
                return BadRequest("Informe uma data válida (YYYY-MM-DD).");

            var flights = await _flightService.GetAvailableFlightsAsync(date);
            if (!flights.Any())
                return NotFound("Nenhum voo disponível para a data informada.");

            return Ok(flights);
        }
    }
}
