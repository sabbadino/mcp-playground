using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;

namespace weather_mcp_server_dapr
{
    [McpServerToolType]
    public sealed class WeatherMcpTool 
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public WeatherMcpTool(IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }   
        [McpServerTool, Description("returns the current weather given a town or region name")]
        public async Task<GetWeatherResponse> GetWeather([Description("The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one")]  string location) {

            var client = _httpClientFactory.CreateClient();
            var ret = await client.GetAsync($"http://api.weatherstack.com/current?access_key={_configuration["apiKey"]}&query={location}&units=m");
            if (!ret.IsSuccessStatusCode)
            {
                throw new Exception($"{ret.StatusCode} + {await ret.Content.ReadAsStringAsync()}");
            }
            return JsonSerializer.Deserialize<GetWeatherResponse>(await ret.Content.ReadAsStreamAsync()) ?? new();
        }
    }

    public class Current
    {
        public string observation_time { get; set; }
        public float temperature { get; set; }
        public int weather_code { get; set; }
        public List<string> weather_icons { get; set; }
        public List<string> weather_descriptions { get; set; }
        public float wind_speed { get; set; }
        public float wind_degree { get; set; }
        public string wind_dir { get; set; }
        public float pressure { get; set; }
        public float precip { get; set; }
        public float humidity { get; set; }
        public float cloudcover { get; set; }
        public float feelslike { get; set; }
        public float uv_index { get; set; }
        public float visibility { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string timezone_id { get; set; }
        public string localtime { get; set; }
        public int localtime_epoch { get; set; }
        public string utc_offset { get; set; }
    }

    public class Request
    {
        public string type { get; set; }
        public string query { get; set; }
        public string language { get; set; }
        public string unit { get; set; }
    }

    public class GetWeatherResponse
    {
        public Request request { get; set; }
        public Location location { get; set; }
        public Current current { get; set; }
    }

}
