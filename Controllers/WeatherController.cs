using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;
    private static int _requestCounter = 0;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet]
    [Route("/weather")] // This makes it accessible at /weather instead of /api/weather
    public async Task<ActionResult<WeatherForecast>> GetWeather(
        [FromQuery] double lat, 
        [FromQuery] double lon)
    {
        // Simulate upstream failures - every 5th request returns 503
        Interlocked.Increment(ref _requestCounter);
        if (_requestCounter % 5 == 0)
        {
            _logger.LogWarning("Simulating upstream failure for request #{RequestNumber}", _requestCounter);
            return StatusCode(503, new { error = "Service temporarily unavailable" });
        }

        try
        {
            if (lat < -90 || lat > 90)
            {
                return BadRequest(new { error = "Latitude must be between -90 and 90" });
            }

            if (lon < -180 || lon > 180)
            {
                return BadRequest(new { error = "Longitude must be between -180 and 180" });
            }

            var forecast = await _weatherService.GetWeatherForecastAsync(lat, lon);

            if (forecast == null)
            {
                return Problem("Failed to retrieve weather data");
            }

            return Ok(forecast);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid request parameters: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Error fetching weather data: {Message}", ex.Message);
            return Problem("Unable to fetch weather data from external service");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while getting weather");
            return Problem("An unexpected error occurred");
        }
    }
}