using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weather_mcp_server_dapr.dtos;

namespace weather_mcp_server_dapr.Controllers
{
    // this controller is for testing purpouses only. It is not required to have the mcp working
    [ApiController]
    [Route("[controller]")]
    public class CurrentWeatherController : ControllerBase
    {
       

        private readonly ILogger<CurrentWeatherController> _logger;
        private readonly IWeatherApiProxy _weatherApiProxy;

        public CurrentWeatherController(ILogger<CurrentWeatherController> logger, IWeatherApiProxy weatherApiProxy)
        {
            _logger = logger;
            _weatherApiProxy = weatherApiProxy;
        }
        //https://mfwmw9kw-7025.euw.devtunnels.ms/CurrentWeather/get-weather?location=london
        //https://localhost:7025/CurrentWeather/get-weather?location=london
        [HttpGet(template:"get-weather", Name = "GetCurrentWeather")]
        public async Task<GetWeatherResponse> GetCurrentWeather([FromQuery] string location)
        {
          return await _weatherApiProxy.GetWeather(location);   
        }

      

        
    }
}
