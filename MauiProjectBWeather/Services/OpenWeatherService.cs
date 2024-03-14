using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using MauiProjectBWeather.Models;
using System.Collections.Concurrent;

namespace MauiProjectBWeather.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();
        //Your API Key
        readonly string apiKey = "2c4b136042f0d448c152ee1a9ea61886";

        public async Task<Forecast> GetForecastAsync(string City)
        {

            Forecast forecast;

            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            if (!cachedCityForecasts.TryGetValue((City, date), out forecast))
            {

                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";



                forecast = await ReadWebApiAsync(uri);

                cachedCityForecasts[(City, DateTime.Now.ToString(date))] = forecast;

                return forecast;


            }

       
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);
            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            var forecast = new Forecast()
            {
                City = wd.city.name,
                Items = wd.list.Select(wdle => new ForecastItem()
                {
                    DateTime = UnixTimeStampToDateTime(wdle.dt),
                    Temperature = wdle.main.temp,
                    WindSpeed = wdle.wind.speed,
                    Description = wdle.weather.First().description,
                    Icon = $"https://openweathermap.org/img/w/{wdle.weather.First().icon}.png"
                }).ToList()
            };
            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
