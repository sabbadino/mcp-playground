
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
                    var roots = await mcpServer.RequestRootsAsync(new ModelContextProtocol.Protocol.Types.ListRootsRequestParams( ) { }, cancellationToken: CancellationToken.None);
                    if(roots.Roots is not null && roots.Roots.Count > 0) {
                        Console.WriteLine("Roots found: " + roots.Roots.Count);
                        foreach (var root in roots.Roots)
                        {
                            Console.WriteLine($"Root: {root.Name} - {root.Uri}");
                            // : comes as %3A in the URI, so we need to replace it  
                            // to investigate 
                            var path = new Uri(root.Uri.Replace("%3A",":")).LocalPath; 
                            File.WriteAllText($"{Path.Combine(path,DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"))}.txt", $"Weather data for {location} at {DateTime.Now}");  
                        }
                    }
                    else
                    {
                        Console.WriteLine("No roots found.");
                    }

                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.ToString());   
                }
            }


          
            return JsonSerializer.Serialize(new {data = "nice weather"});
        }
    }
}
