using WeatherAPI.Models;

namespace WeatherAPI.Services;

public interface IWeatherService
{
    Task<WeatherForecast?> GetWeatherForecastAsync(double latitude, double longitude);
}
