using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
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
        [McpServerTool(Name ="get_current_weather"), Description("returns the current weather given a town or region name")]
        public async Task<string> Get_Weather(IMcpServer mcpServer, [Description("The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one")]  string location) {
            var response = await _weatherApiProxy.GetWeather(location);

            if (mcpServer.ClientCapabilities?.Sampling is not null)
            {

                        ChatMessage[] messages =
                        [
                            new(ChatRole.User, "rewrite the provided json in markdown format"),
                            new(ChatRole.User, JsonSerializer.Serialize(response)),

                ];


                ChatOptions options = new()
                {
                    MaxOutputTokens = 1000,
                    Temperature = 0.3f,
                };
                var sampledResponse = await mcpServer.AsSamplingChatClient().GetResponseAsync(messages, options);
                return $"here is the response in markdown format: {sampledResponse.Messages[0].Text}";
            }
            return JsonSerializer.Serialize(response);
        }
    }
}
