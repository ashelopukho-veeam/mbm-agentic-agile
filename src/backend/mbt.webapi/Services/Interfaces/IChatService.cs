using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Services.Interfaces;

public interface IChatService : IBaseService
{
    Chat GetChatByParentId(string id);
    Task<Chat> GetChatByParentIdAsync(string id);
    ChatMessage AddSystemChatMessage(string chatId, string message);
    Task<ChatMessage> AddSystemChatMessageAsync(string chatId, string message);
    ChatMessage AddChatMessage(string chatId, ChatMessage message);
    Task<ChatMessage> AddChatMessageAsync(string chatId, ChatMessage message);
}
