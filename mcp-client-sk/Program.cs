using Microsoft.SemanticKernel;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Options;
using SkRestApiV1;
using Microsoft.Extensions.Configuration;
using SkRestApiV1.Controllers;
using System.Collections.Immutable;
using Microsoft.OpenApi.Models;

using mcp_shared.ChatGptBot.Ioc;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
#pragma warning disable SKEXP0070
#pragma warning disable SKEXP0001

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOptions<SemanticKernelsSettings>()
    .BindConfiguration(nameof(SemanticKernelsSettings))
    //.ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions
    <SemanticKernelsSettings>, SemanticKernelSettingsValidation>();

var semanticKernelSettingsSection = builder.Configuration.GetSection(nameof(SemanticKernelsSettings));
var semanticKernelSettings = semanticKernelSettingsSection.Get<SemanticKernelsSettings>();
if (semanticKernelSettings == null)
{
    throw new Exception("semanticKernelSettings == null");
}
if (semanticKernelSettings.Kernels.Count == 0)
{
    throw new Exception("No models found in Kernels configuration");
}

builder.Services.RegisterByConvention<Program>();
builder.Services.AddHttpClient();
var allPlugins = semanticKernelSettings.Kernels.SelectMany(k => k.Plugins).Distinct();
foreach (var pluginName in allPlugins)
{
    // build plugin using the global IOC container  
    builder.Services.AddKeyedSingleton(pluginName, (serviceProvider, _) =>
    {
        var type = Type.GetType($"SkRestApiV1.Plugins.{pluginName}");
        ArgumentNullException.ThrowIfNull(type, $"Plugin {pluginName} not found");
        return KernelPluginFactory.CreateFromType(type, pluginName, serviceProvider);
    });
}
// register by convention inside in the default asp.net core ServiceCollection
builder.Services.RegisterByConvention<Program>();

foreach (var kernelSetting in semanticKernelSettings.Kernels)
{
    //it is suested to use AddTransient, but i stick to AddSingleton
    //if you use AddTransient i's beter to create the mcclients upfront out of the lambda below
    // or they will be created on each request 
    builder.Services.AddSingleton(globalServiceProvider =>  {
        // I do not new the Kernel since i need to call AddOpenAIChatCompletion and similar
        var skBuilder = Kernel.CreateBuilder();
        foreach (var model in kernelSetting.Models.Where(m => m.Category == ModelCategory.OpenAi))
        {
            var apiKeyName = builder.Configuration[model.ApiKeyName];
            if (string.IsNullOrEmpty(apiKeyName))
            {
                throw new Exception($"Could not find value for key {apiKeyName}");
            }
            skBuilder.AddOpenAIChatCompletion(model.ModelName, apiKeyName, serviceId: model.ServiceId);
        }
        foreach (var model in kernelSetting.Models.Where(m => m.Category == ModelCategory.Ollama))
        {
            skBuilder.AddOllamaChatCompletion(model.ModelName, new Uri(model.Url), serviceId: model.ServiceId);
        }
        foreach (var model in kernelSetting.Models.Where(m => m.Category == ModelCategory.Gemini))
        {
            var apiKeyName = builder.Configuration[model.ApiKeyName];
            if (string.IsNullOrEmpty(apiKeyName))
            {
                throw new Exception($"Could not find value for key {apiKeyName}");
            }
            skBuilder.AddGoogleAIGeminiChatCompletion(model.ModelName, apiKeyName, serviceId: model.ServiceId);
        }

        skBuilder.Services.AddLogging(l => l.SetMinimumLevel(LogLevel.Debug).AddConsole());
        var kernel = skBuilder.Build();
        foreach (var pluginName in kernelSetting.Plugins)
        {
            var plugin = globalServiceProvider.GetRequiredKeyedService<KernelPlugin>(pluginName);
            ArgumentNullException.ThrowIfNull(plugin, $"Plugin {pluginName} could not be cast to KernelPlugin");
            kernel.Plugins.Add(plugin);
        }
        foreach (var mcpPlugins in kernelSetting.McpPlugins)
        {
            var transport = new SseClientTransport(new SseClientTransportOptions { Endpoint = new Uri(mcpPlugins.Url), UseStreamableHttp = true });
            var mcpClient = McpClientFactory.CreateAsync(transport).Result;
            var tools = mcpClient.ListToolsAsync().Result;
            tools = tools.Where(t => mcpPlugins.AcceptedTools.Contains(t.Name) || mcpPlugins.AcceptedTools.Contains("*")).ToList();   
            kernel.Plugins.AddFromFunctions(mcpPlugins.AsSkPluginNamed, tools.Select(aiFunction => aiFunction.AsKernelFunction()));
        }
        return new KernelWrapper { SystemMessageName = kernelSetting.SystemMessageName, Kernel = kernel, Name = kernelSetting.Name, ServiceIds = kernelSetting.Models.Select(m => m.ServiceId).ToImmutableList() };
    });


}


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