using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using weather_mcp_server_dapr.dtos;

namespace weather_mcp_server_dapr
{
    [McpServerToolType]
    public sealed class WeatherMcpTool 
    {
        private readonly IWeatherApiProxy _weatherApiProxy;

        public WeatherMcpTool(IWeatherApiProxy weatherApiProxy)
        {
            _weatherApiProxy = weatherApiProxy;
        }   
        [McpServerTool, Description("returns the current weather given a town or region name")]
        public async Task<GetWeatherResponse> GetWeather([Description("The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one")]  string location) {

            return await _weatherApiProxy.GetWeather(location);
        }
    }

  

}
