using Google.Api;
using Microsoft.Extensions.DependencyInjection;
using weather_mcp_server_dapr;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Logs;
using mcp_shared.ChatGptBot.Ioc;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr();
builder.Services
    .AddMcpServer().WithHttpTransport(o =>
    {
        
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
//builder.Services.AddOpenTelemetry()
//    .WithTracing(b => b.AddSource("*")
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation().AddConsoleExporter())
//    .WithMetrics(b => b.AddMeter("*")
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation().AddConsoleExporter())
//    .WithLogging(b => b.AddConsoleExporter());
builder.Services.RegisterByConvention<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
#if !DEBUG
app.UseHttpsRedirection();
#endif
app.UseAuthorization();

app.MapControllers();

app.MapMcp();

app.Run();
