using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

public class AddChatMessageStep : StepBody
{
    private readonly IChatService _chatSerivce;

    public AddChatMessageStep(IChatService chatService)
    {
        _chatSerivce = chatService;
    }

    public string Message { get; set; }
    public string ChatParentId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        _chatSerivce.AddSystemChatMessage(ChatParentId, Message);
        return ExecutionResult.Next();
    }
}
