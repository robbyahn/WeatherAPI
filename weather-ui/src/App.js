import React, { useState } from 'react';
import { Cloud, Sun, CloudRain, Snowflake, Wind, Thermometer, Loader2, MapPin } from 'lucide-react';

const WeatherApp = () => {
  const [weather, setWeather] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [coordinates, setCoordinates] = useState({
    lat: -41.2924, // Wellington CBD default
    lon: 174.7787
  });

  const getWeatherIcon = (condition) => {
    switch (condition) {
      case 'Sunny':
        return <Sun className="w-16 h-16 text-yellow-500" />;
      case 'Rainy':
        return <CloudRain className="w-16 h-16 text-blue-500" />;
      case 'Snowing':
        return <Snowflake className="w-16 h-16 text-blue-200" />;
      case 'Windy':
        return <Wind className="w-16 h-16 text-gray-500" />;
      default:
        return <Cloud className="w-16 h-16 text-gray-400" />;
    }
  };

  const getBackgroundClass = (condition) => {
    switch (condition) {
      case 'Sunny':
        return 'from-yellow-400 to-orange-500';
      case 'Rainy':
        return 'from-gray-600 to-blue-600';
      case 'Snowing':
        return 'from-blue-200 to-gray-400';
      case 'Windy':
        return 'from-gray-400 to-gray-600';
      default:
        return 'from-blue-400 to-blue-600';
    }
  };

  const fetchWeather = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch(`http://localhost:5069/weather?lat=${coordinates.lat}&lon=${coordinates.lon}`);
      
      if (response.status === 503) {
        throw new Error('Service temporarily unavailable (simulated failure)');
      }
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const data = await response.json();
      setWeather(data);
    } catch (err) {
      setError(err.message);
      setWeather(null);
    } finally {
      setLoading(false);
    }
  };

  const presetLocations = [
    { name: 'Wellington CBD', lat: -41.2924, lon: 174.7787 },
    { name: 'Auckland', lat: -36.8485, lon: 174.7633 },
    { name: 'Christchurch', lat: -43.5321, lon: 172.6362 },
    { name: 'London', lat: 51.5074, lon: -0.1278 },
  ];

  const setPresetLocation = (location) => {
    setCoordinates({ lat: location.lat, lon: location.lon });
  };

  return (
    <div className={`min-h-screen bg-gradient-to-br ${weather ? getBackgroundClass(weather.condition) : 'from-blue-400 to-blue-600'} p-6`}>
      <div className="max-w-md mx-auto">
        <div className="bg-white/20 backdrop-blur-lg rounded-3xl shadow-2xl p-8 border border-white/30">
          <h1 className="text-3xl font-bold text-white text-center mb-8 flex items-center justify-center gap-2">
            <MapPin className="w-8 h-8" />
            Weather Check
          </h1>

          {/* Location Input */}
          <div className="mb-6">
            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <label className="block text-white/80 text-sm font-medium mb-2">
                  Latitude
                </label>
                <input
                  type="number"
                  value={coordinates.lat}
                  onChange={(e) => setCoordinates(prev => ({ ...prev, lat: parseFloat(e.target.value) }))}
                  step="0.0001"
                  className="w-full px-3 py-2 bg-white/20 border border-white/30 rounded-lg text-white placeholder-white/60 focus:outline-none focus:ring-2 focus:ring-white/40"
                  placeholder="-41.2924"
                />
              </div>
              <div>
                <label className="block text-white/80 text-sm font-medium mb-2">
                  Longitude
                </label>
                <input
                  type="number"
                  value={coordinates.lon}
                  onChange={(e) => setCoordinates(prev => ({ ...prev, lon: parseFloat(e.target.value) }))}
                  step="0.0001"
                  className="w-full px-3 py-2 bg-white/20 border border-white/30 rounded-lg text-white placeholder-white/60 focus:outline-none focus:ring-2 focus:ring-white/40"
                  placeholder="174.7787"
                />
              </div>
            </div>

            {/* Preset Locations */}
            <div className="mb-4">
              <p className="text-white/80 text-sm font-medium mb-2">Quick locations:</p>
              <div className="flex flex-wrap gap-2">
                {presetLocations.map((location) => (
                  <button
                    key={location.name}
                    onClick={() => setPresetLocation(location)}
                    className="px-3 py-1 text-xs bg-white/20 hover:bg-white/30 border border-white/30 rounded-full text-white transition-all duration-200"
                  >
                    {location.name}
                  </button>
                ))}
              </div>
            </div>

            <button
              onClick={fetchWeather}
              disabled={loading}
              className="w-full bg-white/20 hover:bg-white/30 disabled:bg-white/10 border border-white/30 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-200 flex items-center justify-center gap-2"
            >
              {loading ? (
                <>
                  <Loader2 className="w-5 h-5 animate-spin" />
                  Checking Weather...
                </>
              ) : (
                'Get Weather'
              )}
            </button>
          </div>

          {/* Error Display */}
          {error && (
            <div className="mb-6 p-4 bg-red-500/20 border border-red-400/30 rounded-xl">
              <p className="text-white text-sm">
                <strong>Error:</strong> {error}
              </p>
            </div>
          )}

          {/* Weather Display */}
          {weather && (
            <div className="space-y-6">
              {/* Weather Icon and Condition */}
              <div className="text-center">
                <div className="flex justify-center mb-4">
                  {getWeatherIcon(weather.condition)}
                </div>
                <h2 className="text-2xl font-bold text-white mb-2">
                  {weather.condition}
                </h2>
              </div>

              {/* Temperature and Wind */}
              <div className="grid grid-cols-2 gap-4">
                <div className="bg-white/20 rounded-2xl p-4 text-center border border-white/30">
                  <Thermometer className="w-8 h-8 text-white mx-auto mb-2" />
                  <p className="text-2xl font-bold text-white">
                    {weather.temperature}°C
                  </p>
                  <p className="text-white/80 text-sm">Temperature</p>
                </div>
                <div className="bg-white/20 rounded-2xl p-4 text-center border border-white/30">
                  <Wind className="w-8 h-8 text-white mx-auto mb-2" />
                  <p className="text-2xl font-bold text-white">
                    {weather.windSpeed} km/h
                  </p>
                  <p className="text-white/80 text-sm">Wind Speed</p>
                </div>
              </div>

              {/* Recommendation */}
              <div className="bg-white/20 rounded-2xl p-6 border border-white/30">
                <h3 className="text-lg font-semibold text-white mb-3">
                  Recommendation
                </h3>
                <p className="text-white text-base leading-relaxed">
                  {weather.recommendation}
                </p>
              </div>
            </div>
          )}
        </div>

        {/* Footer */}
        <div className="text-center mt-6">
          <p className="text-white/60 text-sm">
            Wellington weather is famously unpredictable! 🌦️
          </p>
        </div>
      </div>
    </div>
  );
};

export default WeatherApp;