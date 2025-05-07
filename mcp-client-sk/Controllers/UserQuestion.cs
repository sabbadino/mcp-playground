#pragma warning disable SKEXP0001

namespace SkRestApiV1.Controllers
{
    public class UserQuestion   
    {
        public string UserPrompt { get; init; } = "";
        public string KernelName { get; init; } = "";
        public string ServiceId{ get; init; } = "";
        public Guid ConversationId { get; init; } = Guid.NewGuid(); 
    }
}
