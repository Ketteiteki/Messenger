using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Dialogs.Command;

public class DeleteDialogCommandHandler : IRequestHandler<DeleteDialogCommand, ChatDto>
{
	private readonly DatabaseContext _context;

	public DeleteDialogCommandHandler(DatabaseContext context)
	{
		_context = context;
	} 
	
	public async Task<ChatDto> Handle(DeleteDialogCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);
		
		if (chatUser == null)
			throw new ForbiddenException("Dialog not found");
		
		var deletedDialogByUsers = await _context.DeletedDialogByUsers
			.Where(d => d.ChatId == request.ChatId && d.Chat.Type == ChatType.Dialog)
			.ToListAsync(cancellationToken);
		
		if (request.IsForBoth || deletedDialogByUsers.Count == 1)
		{
			_context.DeletedDialogByUsers.RemoveRange(deletedDialogByUsers);
			_context.ChatUsers.Remove(chatUser);
			
			await _context.SaveChangesAsync(cancellationToken);

			return new ChatDto
			{
				Id = chatUser.Chat.Id,
				Name = chatUser.Chat.Name,
				Title = chatUser.Chat.Title,
				Type = chatUser.Chat.Type,
				AvatarLink = null,
				MembersCount = 2,
				CanSendMedia = false,
				IsMember = false
			};
		}

		var deleteDialogByUser = new DeletedDialogByUser
		{
			ChatId = request.ChatId,
			UserId = request.RequesterId
		};

		_context.DeletedDialogByUsers.Add(deleteDialogByUser);
		await _context.SaveChangesAsync(cancellationToken);
		
		return new ChatDto
		{
			Id = chatUser.Chat.Id,
			Name = chatUser.Chat.Name,
			Title = chatUser.Chat.Title,
			Type = chatUser.Chat.Type,
			AvatarLink = null,
			MembersCount = 2,
			CanSendMedia = false,
			IsMember = false
		};
	}
}