using mcp_shared.ChatGptBot.Ioc;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.RegisterByConvention<Program>();

var modelName = "gpt-4o";
var openAIApiKey = builder.Configuration["open-ai-api-key"];
var client = new OpenAI.OpenAIClient(openAIApiKey);
builder.Services.AddSingleton(client.GetChatClient(modelName));
var transport = new SseClientTransport(new SseClientTransportOptions { Endpoint = new Uri("http://localhost:5062"), UseStreamableHttp = true });
var mcpClient = await McpClientFactory.CreateAsync(transport);
builder.Services.AddSingleton(mcpClient);
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
