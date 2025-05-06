namespace mcp_client.Controllers
{
    public class Question
    {
        public string Text { get; init; } = "";
        public Guid ConversationId { get; init; } = Guid.NewGuid();
    }
}