# Wellington Weather API
Wellington weather is famously unpredictable. As a developer heading into the office, it helps to know what you’re in for - and what to bring with you.

Your task is to build a small weather-checking API using .NET 8. The API should return a basic forecast along with a helpful recommendation for what to wear.

The endpoint should accept a GET request to /weather, with latitude and longitude as query parameters. When called, it should return a 200 OK response with the following:
- The current temperature in Celsius
- The wind speed in kilometers per hour
- A simple weather condition (Sunny, Windy, Rainy, or Snowing)
- A recommendation phrase suggesting what to wear based on the weather

You’ll need to fetch real data using OpenWeatherMap. The free tier is fine for this exercise.

The recommendation should be based on the forecast and should adhere to the following rules:
On a sunny day you should return "Don't forget to bring a hat".
If it’s over 25°C you should return "It’s a great day for a swim".
If it’s less than 15°C and either raining or snowing you should return "Don't forget to bring a coat".
If it’s raining you should return "Don’t forget the umbrella".
To simulate occasional upstream failures, every fifth request to your endpoint should return a 503 Service Unavailable.

Include tests that cover the different kinds of responses - both success and error cases.

## Bonus Challenge
You've been using this API via cURL or Postman, but it's time for a proper interface.

As an optional bonus, build a small React app that calls your API and displays the weather and recommendation in a simple, user-friendly way.

Extra marks will be given based on design decisions and adherence to clean architecture.

## How to run this demo
Go to Project folder
$ docnet run

Weather UI
Navigate to the /weather-ui
$ npm start 

## Result
![Result](https://github.com/robbyahn/WeatherAPI/blob/main/screenshot.png?raw=true)



## Weather API JSON structure

{
  "coord": {
    "lon": 174.7756,
    "lat": -41.2866
  },
  "weather": [
    {
      "id": 800,
      "main": "Clear",
      "description": "clear sky",
      "icon": "01d"
    }
  ],
  "base": "stations",
  "main": {
    "temp": 11.71,
    "feels_like": 10.71,
    "temp_min": 11.71,
    "temp_max": 11.71,
    "pressure": 998,
    "humidity": 68,
    "sea_level": 998,
    "grnd_level": 986
  },
  "visibility": 10000,
  "wind": {
    "speed": 7.93,
    "deg": 310,
    "gust": 11.7
  },
  "clouds": {
    "all": 0
  },
  "dt": 1756494433,
  "sys": {
    "country": "NZ",
    "sunrise": 1756493385,
    "sunset": 1756533241
  },
  "timezone": 43200,
  "id": 2179537,
  "name": "Wellington",
  "cod": 200
}
