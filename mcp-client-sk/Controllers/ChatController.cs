using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using OpenAI.Chat;
using mcp_shared;
#pragma warning disable SKEXP0001

namespace SkRestApiV1.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatController : ControllerBase
    {

        private static readonly Dictionary<Guid, ChatHistory> _AllMmessages = new();
        private readonly ILogger<ChatController> _logger;
        private readonly SemanticKernelsSettings _semanticKernelSettings;
        private readonly IEnumerable<KernelWrapper> _kernelWrappers;
        private readonly ITemplatesProvider _templatesProvider;

        public ChatController(ILogger<ChatController> logger, IOptions<SemanticKernelsSettings> semanticKernelSettings,
            IEnumerable<KernelWrapper> kernelWrappers, ITemplatesProvider templatesProvider)
        {
            _logger = logger;
            _semanticKernelSettings = semanticKernelSettings.Value;
            _kernelWrappers = kernelWrappers;
            _templatesProvider = templatesProvider;
        }

        [HttpPost(template:"ask", Name = "Ask")]
        public async Task<ActionResult<ResponseToUser>> Ask([FromBody] UserQuestion question)
        {
            if(string.IsNullOrEmpty(question.KernelName) && !string.IsNullOrEmpty(question.ServiceId))
            {
                return BadRequest($"ServiceId {question.KernelName} is not valid without a KernelName"); 
            }

            var defaultKernelName = _semanticKernelSettings.Kernels.Single(k => k.IsDefault).Name;
            var kernelWrapper = _kernelWrappers.SingleOrDefault(k => k.Name == defaultKernelName);
            ArgumentNullException.ThrowIfNull(kernelWrapper, $"Default kernel {defaultKernelName} not found");
            var promptExecutionSettings = new PromptExecutionSettings ();
            promptExecutionSettings.FunctionChoiceBehavior = FunctionChoiceBehavior.Auto();
            string? serviceId = null;
            if (!string.IsNullOrWhiteSpace(question.KernelName)) {
                kernelWrapper = _kernelWrappers.SingleOrDefault(k => k.Name == question.KernelName);
                if (kernelWrapper!=null)
                {
                    if (!string.IsNullOrWhiteSpace(question.ServiceId))
                    {
                        var modelId = kernelWrapper.ServiceIds.SingleOrDefault(m => m == question.ServiceId);
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

            ChatHistory? history = await GetOrCreateConversation(question,kernelWrapper);
            

            history.AddUserMessage(question.UserPrompt);

            // responses are automatically added to the history passed as input 
            var response = await k.GetChatMessageContentAsync(history, promptExecutionSettings, kernelWrapper.Kernel);
           

            return new ResponseToUser { ConversationId = question.ConversationId, Text = response.ToString() };
        }
        private async Task <ChatHistory> GetOrCreateConversation(UserQuestion question, KernelWrapper kernelWrapper)
        {
            _AllMmessages.TryGetValue(question.ConversationId, out var history);
            if (history == null)
            {
                history = new();
                history.AddSystemMessage(await _templatesProvider.GetSystemMessage(kernelWrapper.SystemMessageName));
                _AllMmessages.Add(question.ConversationId, history);
            }

            return history;
        }
    }
}
