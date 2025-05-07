using mcp_shared.ChatGptBot.Ioc;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;
using OpenAI;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.RegisterByConvention<Program>();

var modelName = "gpt-4o";
var openAIApiKey = builder.Configuration["open-ai-api-key"];
var client = new OpenAI.OpenAIClient(openAIApiKey);
var chatClient = client.GetChatClient(modelName);
var samplingClient = chatClient.AsIChatClient();
builder.Services.AddSingleton(chatClient);
var useStreamableHttp = builder.Configuration["UseStreamableHttp"] ?? "true";
var sse = "";
if(useStreamableHttp!="true")
{
    sse = "/sse";
}
var transport = new SseClientTransport(new SseClientTransportOptions { Endpoint = new Uri($"{builder.Configuration["mcp-server"]}{sse}"), UseStreamableHttp = useStreamableHttp != "true" ? false:true});

var mcpClient = await McpClientFactory.CreateAsync(transport,new McpClientOptions { Capabilities = new ClientCapabilities { 
    Sampling = new SamplingCapability() { SamplingHandler = samplingClient.CreateSamplingHandler() } } });


builder.Services.AddSingleton(mcpClient);
builder.Services.RegisterByConvention<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
