namespace WeatherAPI.Models;

public record WeatherForecast(
    double Temperature,
    double WindSpeed,
    string Condition,
    string Recommendation
);
