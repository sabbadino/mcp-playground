using mcp_shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using OpenAI.Chat;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace mcp_client.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {

        private static readonly Dictionary<Guid, List<ChatMessage>> _AllMmessages = new();
        private readonly ILogger<ChatController> _logger;
        private readonly ChatClient _chatClient;
        private readonly IMcpClient _mcpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ChatController(ILogger<ChatController> logger, ChatClient chatClient, IMcpClient mcpClient, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _chatClient = chatClient;
            _mcpClient = mcpClient;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [HttpGet(template: "resources", Name = "resources")]
        public string ListResources()
        {
            var sb = new StringBuilder();
            var resources = _mcpClient.EnumerateResourcesAsync();
            foreach (var resource in resources.ToBlockingEnumerable())
            {
                sb.AppendLine(resource.Uri);
            }
            return sb.ToString();
        }


        [HttpGet(template: "resource", Name = "resource")]
        public async Task<string> ListResources([FromQuery] string name = "file:///c:/Temp")
        {
            var sb = new StringBuilder();
            var resources = await _mcpClient.ReadResourceAsync(name);
            foreach (var content in resources.Contents)
            {
                if (content is TextResourceContents str)
                {
                    sb.AppendLine(str.Text);
                }
            }
            return sb.ToString();
        }

        [HttpPost(template: "ask-mcp", Name = "AskWithMcp")]
        public async Task<ResponseToUser> AskWithMcp(Question question)
        {
            var tools = await _mcpClient.ListToolsAsync();
            List<ChatMessage>? messages = GetOrCreateConversation(question.ConversationId);
            messages.Add(new UserChatMessage(question.Text));
            var co = new ChatCompletionOptions();
            foreach (var tool in tools)
            {
                co.Tools.Add(tool.ToOpenAITool());
            }
            bool requiresAction;

            do
            {
                requiresAction = false;
                ChatCompletion completion = _chatClient.CompleteChat(messages, co);

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        {
                            // Add the assistant message to the conversation history.
                            messages.Add(new AssistantChatMessage(completion));
                            break;
                        }

                    case ChatFinishReason.ToolCalls:
                        {
                            // First, add the assistant message with tool calls to the conversation history.
                            messages.Add(new AssistantChatMessage(completion));

                            // Then, add a new tool message for each tool call that is resolved.
                            foreach (ChatToolCall toolCall in completion.ToolCalls)
                            {
                                if (tools.Select(t => t.Name).Contains(toolCall.FunctionName, StringComparer.OrdinalIgnoreCase))
                                {
                                    var toolResult = await _mcpClient.CallToolAsync(toolCall.FunctionName, JsonSerializer.Deserialize<Dictionary<string, object?>>(toolCall.FunctionArguments.ToString()));
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult.Content[0].Text));
                                }
                                else
                                {
                                    throw new Exception($"Tool {toolCall.FunctionName} not found");
                                }
                            }

                            requiresAction = true;
                            break;
                        }

                    case ChatFinishReason.Length:
                        throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                    case ChatFinishReason.ContentFilter:
                        throw new NotImplementedException("Omitted content due to a content filter flag.");

                    case ChatFinishReason.FunctionCall:
                        throw new NotImplementedException("Deprecated in favor of tool calls.");

                    default:
                        throw new NotImplementedException(completion.FinishReason.ToString());
                }
            } while (requiresAction);

            return new ResponseToUser { Text = messages.Last().Content[0].Text, ConversationId = question.ConversationId };
        }

        [HttpPost(template: "ask", Name = "Ask")]
        public async Task<ResponseToUser> Ask(Question question)
        {
            var weatherTool = getCurrentWeatherTool;
            List<ChatMessage>? messages = GetOrCreateConversation(question.ConversationId);
            messages.Add(new UserChatMessage(question.Text));
            var co = new ChatCompletionOptions();
            co.Tools.Add(weatherTool);
            bool requiresAction;
            do
            {
                // exit the loop unless there is a function call 
                requiresAction = false;
                ChatCompletion completion = _chatClient.CompleteChat(messages, co);

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        {
                            // Add the assistant message to the conversation history.
                            // exit the loop. It is a reply for the human user
                            messages.Add(new AssistantChatMessage(completion));
                            break;
                        }
                    // function calls output from LLM 
                    case ChatFinishReason.ToolCalls:
                        {
                            // First, add the assistant message with tool calls to the conversation history.
                            messages.Add(new AssistantChatMessage(completion));

                            // Then, add a new tool message for each tool call that is resolved.
                            foreach (ChatToolCall toolCall in completion.ToolCalls)
                            {
                                if (co.Tools.Select(t => t.FunctionName).Contains(toolCall.FunctionName, StringComparer.OrdinalIgnoreCase))
                                {
                                    // parse arguments of function call
                                    var args = JsonDocument.Parse(toolCall.FunctionArguments);
                                    var location = args.RootElement.GetProperty("location").ToString();
                                    // make call to data provider 
                                    var toolResult = await GetWeather(location);
                                    // add the response to conversation history.
                                    // In next iteration of the while the LLM will use this information
                                    // to return a response to the user
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                }
                                else
                                {
                                    throw new Exception($"Tool {toolCall.FunctionName} not found");
                                }
                            }
                            // to stay in the loop, so that LLM reply knowing hte function call result 
                            requiresAction = true;
                            break;
                        }
                    default:
                        throw new NotImplementedException(completion.FinishReason.ToString());
                }
            } while (requiresAction);
            // return the last item added in the while loop 
            return new ResponseToUser { Text = messages.Last().Content[0].Text, ConversationId = question.ConversationId };
        }

        private static readonly ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
        functionName: "get_current_weather",
        functionDescription: "returns the current weather given a town or region name",
        functionParameters: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one"
                    }
                },
                "required": [ "location" ]
            }
            """u8.ToArray())
        );

        private static List<ChatMessage> GetOrCreateConversation(Guid conversationId)
        {
            _AllMmessages.TryGetValue(conversationId, out var messages);
            if (messages == null)
            {
                messages = new();
                _AllMmessages.Add(conversationId, messages);
            }

            return messages;
        }


        public async Task<string> GetWeather(string location)
        {

            var client = _httpClientFactory.CreateClient();
            var ret = await client.GetAsync($"http://api.weatherstack.com/current?access_key={_configuration["weatherApiKey"]}&query={location}&units=m");
            if (!ret.IsSuccessStatusCode)
            {
                return $"Error getting weather StatusCode {ret.StatusCode} + {await ret.Content.ReadAsStringAsync()}";
            }
            return await ret.Content.ReadAsStringAsync();
        }

    }
}

 
