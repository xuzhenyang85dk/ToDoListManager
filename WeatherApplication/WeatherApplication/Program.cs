// Learning objectives
// Asynchronous programming with async and await. Async and await without block I/O, HttpClient
// JSON data handling. JObject, ToObject<T>
// API collection skills. Request parameters structure, error response handling, data cache strategy
// User experience optimization. Set default value, data visualization, Exception remind


using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class WeatherApplication
{
    // Weather code lookup table (Open-Meteo standard)
    // Location Denmark Copenhagen (Latitude 55.6761 Longitude 12.5683)
    private const string DefaultLat = "55.6761";
    private const string DefaultLon = "12.5683";

    // Weather code lookup table (Open-Meteo standard)
    // Specify Euro weather description
    private static readonly Dictionary<int, string> WeatherCodes = new Dictionary<int, string>()
    {
        {0,"Clear sky"},
        {1,"Mainly clear"},
        {2,"Partly cloudy"},
        {3,"Overcast"},
        {45,"Fog"},
        {48,"Freazing fog"},
        {51,"Light drizzle"},
        {53,"Moderate drizzle"},
        {55,"Heavy drizzle"},
        {56,"Light freezing rain"},
        {57,"Heavy freezing rain"},
        {61,"Light rain"},
        {63,"Moderate rain"},
        {65,"Heavy rain"},
        {66,"Sleet showers"},
        {67,"Heavy sleet showers"},
        {71,"Light snow"},
        {73,"Moderate snow"},
        {75,"Heavy snow"},
        {77,"Snow grains"},
        {80,"Light showers"},
        {81,"Moderate showers"},
        {82,"Heavy showers"},
        {85,"Light snow showers"},
        {86,"Heavy snow showers"},
        {95,"Thunderstorm"},
        {96,"Thunderstorm with light hail"},
        {99,"Thunderstorm with heavy hail"}
    };

    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(10);

        while (true)
        {
            Console.WriteLine("\n=== Copenhagen Weather Application ===");
            Console.WriteLine("1. Check Weather");
            Console.WriteLine("2. Exit");
            Console.Write("Please select an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await FetchWeatherDate(client);
                    break;
                case "2":
                    return;
                default:
                    Console.WriteLine("Invaild input, please try again");
                    break;
            }
        }
    }

    static async Task FetchWeatherDate(HttpClient client)
    {
        try
        {
            Console.Write($"Enter latitude (default {DefaultLat} for Copenhagen): ");
            string? lat = Console.ReadLine();
            lat = string.IsNullOrWhiteSpace(lat) ? DefaultLat : lat;

            Console.Write($"Enter longtitude (default {DefaultLon} for Copenhagen): ");
            string? lon = Console.ReadLine();
            lon = string.IsNullOrWhiteSpace(lon) ? DefaultLon : lon;

            // Add Euro specify parameters(snowfail probability)
            string apiUrl = $"https://api.open-meteo.com/v1/forecast?" +
                $"latitude={lat}&longitude={lon}" +
                "&current=temperature_2m,weather_code,wind_speed_10m,apparent_temperature" +
                "&hourly=temperature_2m,precipitation_probability,snowfall" +
                "&daily=weather_code,temperature_2m_max,temperature_2m_min" +
                "&timezone=Europe%2FCopenhagen&wind_speed_unit=ms";

            string response = await client.GetStringAsync(apiUrl);

            JObject json = JObject.Parse(response);

            // Today's Forecast
            var current = json["current"];

            int weatherCode = (int)current["weather_code"]!;

            double temperature = (double)current["temperature_2m"]!;

            double feelsLike = (double)current["apparent_temperature"]!;

            double windSpeed = (double)current["wind_speed_10m"]!;

            // Hourly forecast Conditions
            var hourly = json["hourly"]!;

            var times = hourly["time"]!.ToObject<List<string>>()!;

            var temps = hourly["temperature_2m"]!.ToObject<List<double>>()!;

            var rainChange = hourly["precipitation_probability"]!.ToObject<List<int>>()!;

            var snowfall = hourly["snowfall"]!.ToObject<List<double>>()!; // cm unit

            // Daily forecast Conditions
            var daily = json["daily"]!;

            var dailyDates = daily["time"]!.ToObject<List<string>>()!;

            var maxTemps = daily["temperature_2m_max"]!.ToObject<List<double>>()!;

            var minTemps = daily["temperature_2m_min"]!.ToObject<List<double>>()!;

            //Show today's Forecast
            Console.WriteLine("\n=== Current Conditions ===");

            Console.WriteLine($"Weather: {WeatherCodes[weatherCode]}");

            Console.WriteLine($"Temperature: {temperature} C (Feels like {feelsLike} C)");

            Console.WriteLine($"Wind Speed: {windSpeed} m/s");

            // Show 6 hour Forecast
            Console.WriteLine("\n=== 6-Hour Forecast");

            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine($"{times[i].Substring(11)} - " +
                    $"Temp: {temps[i]} C | " +
                    $"Rain: {rainChange[i]}% | " +
                    $"Snow: {snowfall[i]}cm");
            }


            // Show 7-Day Forecast
            Console.WriteLine("\n=== 7-Day Forecast ===");

            for (int i = 0; i < 7; i++)
            {
                Console.WriteLine($"{dailyDates[i]} | " +
                    $"High: {maxTemps[i]}C | " +
                    $"Low: {minTemps[i]}C | " +
                    $"{WeatherCodes[(int)daily["weather_code"]![i]!]}");
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Query failed: {ex.Message}");
        }
    }
}