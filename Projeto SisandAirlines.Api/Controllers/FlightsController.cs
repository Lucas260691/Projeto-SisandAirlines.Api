using Microsoft.AspNetCore.Mvc;
using SisandAirlines.Application.Services;

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
        /// Retorna os voos disponíveis conforme a data e número de passageiros (Curitiba → São Paulo).
        /// </summary>
        /// <param name="date">Data do voo (YYYY-MM-DD)</param>
        /// <param name="passengers">Número opcional de passageiros</param>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableFlights([FromQuery] DateTime date, [FromQuery] int? passengers = null)
        {
            try
            {
                if (date == default)
                    return BadRequest(new { message = "Informe uma data válida (YYYY-MM-DD)." });

                var flights = await _flightService.GetAvailableFlightsAsync(date, passengers);

                if (!flights.Any())
                    return NotFound(new { message = "Nenhum voo disponível para a data informada." });

                return Ok(flights);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao consultar voos: {ex.Message}");
                return StatusCode(500, new { message = "Erro interno ao buscar voos disponíveis." });
            }
        }
    }
}
