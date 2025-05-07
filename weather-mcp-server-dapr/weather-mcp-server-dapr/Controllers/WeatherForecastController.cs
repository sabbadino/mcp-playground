using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weather_mcp_server_dapr.dtos;

namespace weather_mcp_server_dapr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherApiProxy _weatherApiProxy;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherApiProxy weatherApiProxy)
        {
            _logger = logger;
            _weatherApiProxy = weatherApiProxy;
        }

        [HttpPost(template:"get-weather", Name = "GetCurrentWeather")]
        public async Task<GetWeatherResponse> GetCurrentWeather([FromBody] GetWeatherRequest request)
        {
          return await _weatherApiProxy.GetWeather(request.Location);   
        }
    }
}
