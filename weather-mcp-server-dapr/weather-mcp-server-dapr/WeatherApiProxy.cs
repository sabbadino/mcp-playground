using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using mcp_shared.ChatGptBot.Ioc;
using Microsoft.Extensions.Configuration;
using weather_mcp_server_dapr.dtos;

namespace weather_mcp_server_dapr
{
    public class WeatherApiProxy : IWeatherApiProxy,ISingletonScope
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public WeatherApiProxy(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<GetWeatherResponse> GetWeather(string location)
        {

            var client = _httpClientFactory.CreateClient();
            var ret = await client.GetAsync($"http://api.weatherstack.com/current?access_key={_configuration["weatherApiKey"]}&query={location}&units=m");
            if (!ret.IsSuccessStatusCode)
            {
                throw new Exception($"{ret.StatusCode} + {await ret.Content.ReadAsStringAsync()}");
            }
            return JsonSerializer.Deserialize<GetWeatherResponse>(await ret.Content.ReadAsStreamAsync()) ?? new();
        }
    }

    public interface IWeatherApiProxy
    {
        Task<GetWeatherResponse> GetWeather(string location);
    }
}
