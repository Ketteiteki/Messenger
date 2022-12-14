using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class LeaveFromChatCommandHandler : IRequestHandler<LeaveFromChatCommand, Result<ChatDto>>
{
    private readonly DatabaseContext _context;

    public LeaveFromChatCommandHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<ChatDto>> Handle(LeaveFromChatCommand request, CancellationToken cancellationToken)
    {
        var chatUser = await _context.ChatUsers
            .Include(c => c.Chat)
            .ThenInclude(c => c.Owner)
            .FirstOrDefaultAsync(c => 
                c.ChatId == request.ChatId &&
                c.UserId == request.RequesterId &&
                c.Chat.Type != ChatType.Dialog, cancellationToken);

        if (chatUser == null)
        {
            return new Result<ChatDto>(new ForbiddenError("No user found in chat"));
        }
        
        _context.ChatUsers.Remove(chatUser);
        await _context.SaveChangesAsync(cancellationToken);

        return new Result<ChatDto>(
            new ChatDto
            {
                Id = chatUser.ChatId,
                Name = chatUser.Chat.Name,
                Title = chatUser.Chat.Title,
                Type = chatUser.Chat.Type,
                AvatarLink = chatUser.Chat.AvatarLink,
                LastMessageId = chatUser.Chat.LastMessageId,
                LastMessageText = chatUser.Chat.LastMessage?.Text,
                LastMessageAuthorDisplayName = chatUser.Chat.LastMessage != null && chatUser.Chat.LastMessage.Owner != null ?
                    chatUser.Chat.LastMessage.Owner.DisplayName : null,
                LastMessageDateOfCreate = chatUser.Chat.LastMessage?.DateOfCreate,
                CanSendMedia = chatUser.CanSendMedia,
                IsOwner = chatUser.Chat.OwnerId == request.RequesterId,
                IsMember = false,
                MuteDateOfExpire = chatUser.MuteDateOfExpire,
                BanDateOfExpire = null
            });
    }
}