using System;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MongoDB.Bson;

namespace mbt.webapi.Services;

public class ChatService : IChatService
{
    private readonly IDbBaseRepository<Chat> _chatRepository;

    public ChatService(IDbBaseRepository<Chat> chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public Chat GetChatByParentId(string id)
    {
        var c = _chatRepository.FindOne(c => c.ParentId == id);
        return c;
    }

    public Task<Chat> GetChatByParentIdAsync(string id)
    {
        return _chatRepository.FindOneAsync(c => c.ParentId == id);
    }

    public ChatMessage AddSystemChatMessage(string chatId, string message)
    {
        var chatMsg = new ChatMessage
        {
            Title = message,
            CreatedBy = BuiltInConstants.SystemUserId
        };
        return AddChatMessage(chatId, chatMsg);
    }

    public Task<ChatMessage> AddSystemChatMessageAsync(string chatId, string message)
    {
        var chatMsg = new ChatMessage
        {
            Title = message,
            CreatedBy = BuiltInConstants.SystemUserId
        };
        return AddChatMessageAsync(chatId, chatMsg);
    }

    public ChatMessage AddChatMessage(string chatId, ChatMessage message)
    {
        var c = GetChatByParentId(chatId);
        if (c == null)
        {
            c = new Chat
            {
                ParentId = chatId
            };
            _chatRepository.Create(c);
        }

        message.Created = DateTime.Now;
        message.Id = ObjectId.GenerateNewId().ToString();

        c.Messages.Add(message);
        _chatRepository.UpdateAsync(c);

        return message;
    }

    public async Task<ChatMessage> AddChatMessageAsync(string chatId, ChatMessage message)
    {
        var c = await GetChatByParentIdAsync(chatId);
        if (c == null)
        {
            c = new Chat
            {
                ParentId = chatId
            };
            await _chatRepository.CreateAsync(c);
        }

        message.Created = DateTime.Now;
        message.Id = ObjectId.GenerateNewId().ToString();

        c.Messages.Add(message);
        await _chatRepository.UpdateAsync(c);

        return message;
    }
}
