using Microsoft.AspNetCore.Mvc;

namespace mbt.webapi.Endpoints.Chat;

public class ChatMessagePostData
{
    [FromRoute] public string Id { get; set; }

    [FromBody] public NewMessagePostData NewMessageData { get; set; }
}
