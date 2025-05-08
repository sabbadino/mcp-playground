using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weather_mcp_server_dapr.dtos;

namespace weather_mcp_server_dapr.Controllers
{
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

        [HttpPost(template:"get-weather", Name = "GetCurrentWeather")]
        public async Task<GetWeatherResponse> GetCurrentWeather([FromBody] GetWeatherRequest request)
        {
          return await _weatherApiProxy.GetWeather(request.Location);   
        }

        [HttpGet(template:"get-computer-name", Name ="GetComputerName")]
        public ActionResult<string> GetComputerName()
        {
            try
            {
                string computerName = Environment.MachineName;
                return Ok(computerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the computer name.");
                return StatusCode(500, "An error occurred while retrieving the computer name.");
            }
        }

        
    }
}
