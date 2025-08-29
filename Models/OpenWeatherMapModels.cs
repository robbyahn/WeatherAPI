using System.Text.Json.Serialization;

namespace WeatherAPI.Models;

public record OpenWeatherMapResponse(
    [property: JsonPropertyName("main")] MainData? Main,
    [property: JsonPropertyName("wind")] WindData? Wind,
    [property: JsonPropertyName("weather")] WeatherDescription[]? Weather
);

public record MainData(
    [property: JsonPropertyName("temp")] double Temp
);

public record WindData(
    [property: JsonPropertyName("speed")] double Speed
);

public record WeatherDescription(
    [property: JsonPropertyName("main")] string Main, 
    [property: JsonPropertyName("description")] string Description
);