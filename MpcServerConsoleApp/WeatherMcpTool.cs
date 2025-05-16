
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading;


namespace MpcServerConsoleApp
{
    [McpServerToolType]
    public sealed class WeatherMcpTool 
    {

        public WeatherMcpTool()
        {
            
        }   
        [McpServerTool(Name ="get_current_weather"), 
            Description("returns the current weather given a town or region name")]
        public async Task<string> Get_Weather(IMcpServer mcpServer, 
            [Description("The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one")]  string location) {
            
            if (mcpServer.ClientCapabilities?.Roots is not null)
            {
                try
                {
                    var r = await mcpServer.RequestRootsAsync(new ModelContextProtocol.Protocol.Types.ListRootsRequestParams( ) { }, cancellationToken: CancellationToken.None);
                }
                catch (Exception ex) { 
                Console.WriteLine(ex.ToString());   
                }
            }


          
            return JsonSerializer.Serialize(new {data = "nice weather"});
        }
    }
}
