using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using OpenAI.Chat;
using mcp_shared;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Text.Json;
#pragma warning disable SKEXP0001

namespace SkRestApiV1.Controllers;

[ApiController]
[Route("chat-serialized")]
public class ChatHistorySerializedController : ControllerBase
{

    private static readonly Dictionary<Guid, string> _AllMessages = new();
    private readonly ILogger<ChatController> _logger;
    private readonly SemanticKernelsSettings _semanticKernelSettings;
    private readonly IEnumerable<KernelWrapper> _kernelWrappers;
    private readonly ITemplatesProvider _templatesProvider;

    public ChatHistorySerializedController(ILogger<ChatController> logger, IOptions<SemanticKernelsSettings> semanticKernelSettings,
        IEnumerable<KernelWrapper> kernelWrappers, ITemplatesProvider templatesProvider)
    {
        _logger = logger;
        _semanticKernelSettings = semanticKernelSettings.Value;
        _kernelWrappers = kernelWrappers;
        _templatesProvider = templatesProvider;
    }

    [HttpPost(template: "ask", Name = "Ask2")]
    public async Task<ActionResult<ResponseToUser>> Ask([FromBody] UserQuestion question)
    {
        if (string.IsNullOrEmpty(question.KernelName) && !string.IsNullOrEmpty(question.ServiceId))
        {
            return BadRequest($"ServiceId {question.KernelName} is not valid without a KernelName");
        }

        var defaultKernelName = _semanticKernelSettings.Kernels.Single(k => k.IsDefault).Name;
        var kernelWrapper = _kernelWrappers.SingleOrDefault(k => k.Name == defaultKernelName);

        ArgumentNullException.ThrowIfNull(kernelWrapper, $"Default kernel {defaultKernelName} not found");
        var serviceId = kernelWrapper.Models.SingleOrDefault(m => m.IsDefault)?.ServiceId;    
        if (!string.IsNullOrWhiteSpace(question.KernelName)) {
            kernelWrapper = _kernelWrappers.SingleOrDefault(k => k.Name == question.KernelName);
            if (kernelWrapper!=null)
            {
                if (!string.IsNullOrWhiteSpace(question.ServiceId))
                {
                    var modelId = kernelWrapper.Models.SingleOrDefault(m => m.ServiceId == question.ServiceId);
                    if (modelId!=null)
                    {
                        serviceId = question.ServiceId;
                    }
                    else
                    {
                        return BadRequest($"ServiceId {question.ServiceId} not found");
                    }
                }
            }
            else
            {
                return BadRequest($"KernelName {question.KernelName} not found"); 
            }
        }
      
        var k = kernelWrapper.Kernel.GetRequiredService<IChatCompletionService>(serviceId);

        ChatHistoryWithConversationId? chatHistoryWithGuid = await GetOrCreateConversation(question,kernelWrapper);
        

        chatHistoryWithGuid.History.AddUserMessage(question.UserPrompt);

        // responses are automatically added to the history passed as input 
        var model = kernelWrapper.Models.SingleOrDefault(m => m.ServiceId == serviceId);
        ArgumentNullException.ThrowIfNull(model, $"Model {question.ServiceId} not found in kernel {kernelWrapper.Name}");
        var promptExecutionSettings = new PromptExecutionSettings();
        promptExecutionSettings.FunctionChoiceBehavior = FunctionChoiceBehavior.Auto();
        if (model.Category == ModelCategory.Gemini)
        {
#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var geminiPromptExecutionSettings = new GeminiPromptExecutionSettings();
            geminiPromptExecutionSettings.ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions;
            promptExecutionSettings = geminiPromptExecutionSettings;    
        }
#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        //        }
        //  kernelWrapper.Kernel.Plugins.Clear();   
        var history = chatHistoryWithGuid.History;
        var response = await k.GetChatMessageContentAsync(history, promptExecutionSettings, kernelWrapper.Kernel);
        chatHistoryWithGuid.History.Add(new Microsoft.SemanticKernel.ChatMessageContent { Items = response.Items, Role = response.Role });
        await PersistsConversation(chatHistoryWithGuid);

        return new ResponseToUser { ConversationId = question.ConversationId, Text = response.ToString() };
    }

    private static async Task PersistsConversation(ChatHistoryWithConversationId chatHistoryWithGuid)
    {
        ChatHistoryWithConversationId history;
        if(!_AllMessages.ContainsKey(chatHistoryWithGuid.ConversationId)) { 
            throw new Exception($"Conversation wth Guid {chatHistoryWithGuid.ConversationId} not found");
        }
        else
        {
            _AllMessages[chatHistoryWithGuid.ConversationId] = JsonSerializer.Serialize(chatHistoryWithGuid);
        }
    }

    private async Task <ChatHistoryWithConversationId> GetOrCreateConversation(UserQuestion question, KernelWrapper kernelWrapper)
    {
        ChatHistoryWithConversationId history;    
        _AllMessages.TryGetValue(question.ConversationId, out var historyAsString);
        if (historyAsString == null)
        {
            var chatHistory = new ChatHistory ();
            chatHistory.AddSystemMessage(await _templatesProvider.GetSystemMessage(kernelWrapper.SystemMessageName));
            history = new ChatHistoryWithConversationId { History = chatHistory, ConversationId = question.ConversationId };
            _AllMessages.Add(question.ConversationId, JsonSerializer.Serialize(history));
        }
        else
        {
            history = JsonSerializer.Deserialize<ChatHistoryWithConversationId>(historyAsString);   
            ArgumentNullException.ThrowIfNull(history); 
        }
        return history;
    }
}
public class ChatHistoryWithConversationId
{
    public required Guid ConversationId { get; init; }
    public required ChatHistory History { get; init; } 
    
}   