using System.Collections.Generic;

namespace mbt.webapi.Endpoints.Chat;

public class NewMessagePostData
{
    public string Message { get; set; }
    public string MessageUrl { get; set; }
    public List<string> Mentions { get; set; }
}
