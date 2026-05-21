using System.Collections.Generic;
using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class ChatMessage : BaseItem
{
}

public class Chat : BaseIdItem
{
    public string ParentId { get; set; }
    public string Collection { get; set; } = "";


    public List<ChatMessage> Messages { get; set; } = new();
}

public class ChatMessageExpanded : ChatMessage
{
    public UserProfile CreatedByUser { get; set; }
}

