using System.Text.Json;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        ILogger<WeatherService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<WeatherForecast?> GetWeatherForecastAsync(double latitude, double longitude)
    {
        var apiKey = _configuration["OpenWeatherMap:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentException("OpenWeatherMap API key not configured");
        }

        var httpClient = _httpClientFactory.CreateClient();
        var url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";
        
        try
        {
            var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenWeatherMap API returned {StatusCode}: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();

            var weatherData = JsonSerializer.Deserialize<OpenWeatherMapResponse>(jsonContent);
            _logger.LogInformation("Raw weatherData: {weatherData}", weatherData);


            if (weatherData == null || weatherData.Main == null)
            {
                _logger.LogError("Invalid or incomplete weather data received from API");
                return null;
            }

            return ConvertToWeatherForecast(weatherData);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when calling OpenWeatherMap API");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse JSON response from OpenWeatherMap API");
            return null;
        }
    }

    private static WeatherForecast ConvertToWeatherForecast(OpenWeatherMapResponse weatherData)
    {
        // Handle potential null values
        var windSpeed = weatherData.Wind?.Speed ?? 0;
        var windSpeedKmh = windSpeed * 3.6; // Convert m/s to km/h
        
        var weatherMain = weatherData.Weather?.FirstOrDefault()?.Main ?? "Clear";
        var temperature = weatherData.Main?.Temp ?? 20; // Default to 20°C if null

        return new WeatherForecast(
            Temperature: Math.Round(temperature, 1),
            WindSpeed: Math.Round(windSpeedKmh, 1),
            Condition: MapWeatherCondition(weatherMain, windSpeedKmh),
            Recommendation: GetClothingRecommendation(temperature, windSpeedKmh, weatherMain)
        );
    }

    private static string MapWeatherCondition(string openWeatherMain, double windSpeedKmh)
    {
        // If it's very windy (>25 km/h), prioritize wind condition
        if (windSpeedKmh > 25)
        {
            return "Windy";
        }

        return openWeatherMain switch
        {
            "Clear" => "Sunny",
            "Clouds" => windSpeedKmh > 15 ? "Windy" : "Sunny",
            "Rain" or "Drizzle" => "Rainy",
            "Snow" => "Snowing",
            "Thunderstorm" => "Rainy",
            _ => windSpeedKmh > 15 ? "Windy" : "Sunny"
        };
    }

    private static string GetClothingRecommendation(double tempC, double windSpeedKmh, string weatherMain)
    {
        var recommendations = new List<string>();

        // Base temperature recommendations
        if (tempC < 15)
        {
            recommendations.Add("Don't forget to bring a coat");
        }
        else if (tempC > 25)
        {
            recommendations.Add("It’s a great day for a swim");
        }
        else
        {
            recommendations.Add("light clothing");
        }

        // Wind adjustments
        if (windSpeedKmh > 25)
        {
            recommendations.Add("windproof outer layer");
        }
        else if (windSpeedKmh > 15)
        {
            recommendations.Add("wind-resistant jacket");
        }

        // Weather-specific additions
        switch (weatherMain)
        {
            case "Rain" or "Drizzle" or "Thunderstorm":
                recommendations.Add("Don’t forget the umbrella");
                break;
            case "Snow":
                recommendations.Add("waterproof boots and warm hat");
                break;
        }

        // Wellington-specific advice
        if (windSpeedKmh > 20 || weatherMain is "Rain" or "Drizzle")
        {
            recommendations.Add("(typical Wellington weather!)");
        }

        return $"Wear {string.Join(", ", recommendations)}.";
    }
}