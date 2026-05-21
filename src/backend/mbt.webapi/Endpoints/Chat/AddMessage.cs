using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Jobs;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ChatMessage = mbt.webapi.Domain.Entities.ChatMessage;

namespace mbt.webapi.Endpoints.Chat;

public class AddMessage : EndpointBaseAsync.WithRequest<ChatMessagePostData>.WithActionResult<ChatMessage>
{
    // aaa ({user_name||user_id}) bbb
    private const string MentionsRegexMask = @"\(\{(.*?)\|\|(.*?)\}\)";

    private readonly ICurrentUserContext _currentUserContext;
    private readonly IChatService _chatService;
    private readonly IMailService _mailService;
    private readonly IApiService _apiService;
    private readonly IUserService _userService;

    public AddMessage(ICurrentUserContext currentUserContext, IChatService chatService, IMailService mailService,
        IApiService apiService, IUserService userService)
    {
        _currentUserContext = currentUserContext;
        _chatService = chatService;
        _mailService = mailService;
        _apiService = apiService;
        _userService = userService;
    }

    [HttpPost("api/chat/{Id}")]
    [SwaggerOperation(
        Summary = "Add message to the chat",
        Description = "Add message to the chat",
        OperationId = "Chat.AddMessage",
        Tags = new[] { "Chat" })]
    public override async Task<ActionResult<ChatMessage>> HandleAsync([FromRoute] ChatMessagePostData request,
        CancellationToken cancellationToken = new())
    {
        ChatMessage newMsg = new()
        {
            CreatedBy = _currentUserContext.UserId,
            Title = request.NewMessageData.Message
        };

        var msg = await _chatService.AddChatMessageAsync(request.Id, newMsg);

        if (!string.IsNullOrEmpty(request.NewMessageData.MessageUrl) &&
            request.NewMessageData.Mentions is { Count: > 0 })
            await SendChatNotifications(request.NewMessageData.Message, request.NewMessageData.MessageUrl);
        return msg;
    }

    private async Task SendChatNotifications(string chatMessage, string chatLink)
    {
        var mentions = ExtractMentions(chatMessage, MentionsRegexMask);
        var chatMessageWithReplacedMentions = ReplaceMentions(chatMessage, MentionsRegexMask);

        var host = (await _apiService.GetAppConfigAsync()).ClientHostUrl;
        var chatAbsoluteUrl = UrlUtils.Combine(host, chatLink);

        foreach (var (id, _) in mentions)
        {
            var user = await _userService.GetAsync(id);

            var mailBody = MailTemplates.GetChatNotificationEmail(
                chatMessageWithReplacedMentions,
                chatAbsoluteUrl);

            await _mailService.QueueAsync(user.Email, GroupedMailTemplates.Chat.UserMentioned.Subject, mailBody);
        }
    }

    private static List<(string id, string name)> ExtractMentions(string text, string regexMask)
    {
        var matches = Regex.Matches(text, regexMask);

        var result = matches.Select(r => (id: r.Groups[2].Value, name: r.Groups[1].Value))
            .Distinct()
            .ToList();

        return result;
    }

    private string ReplaceMentions(string text, string regexMask)
    {
        // aaa ({user_id||user_name}) bbb
        var result = Regex.Replace(text, regexMask, m => m.Groups[1].Value);

        return result;
    }
}
