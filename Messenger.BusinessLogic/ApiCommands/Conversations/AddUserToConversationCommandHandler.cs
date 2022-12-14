using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class AddUserToConversationCommandHandler : IRequestHandler<AddUserToConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public AddUserToConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<UserDto>> Handle(AddUserToConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Role)
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<UserDto>(new ForbiddenError("No requester in the chat"));
		}
		
		if (chatUserByRequester.Role is { CanAddAndRemoveUserToConversation: true } ||
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var user = await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

			if (user == null)
			{
				return new Result<UserDto>(new  DbEntityNotFoundError("User not found"));
			}
			
			var chatUserByUser = await _context.ChatUsers
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ChatId == request.ChatId, cancellationToken);

			if (chatUserByUser != null)
			{
				return new Result<UserDto>(new DbEntityExistsError("User already exists in conversation"));
			}

			if (user.ChatUsers.Any(c => c.UserId == request.UserId && c.ChatId == request.ChatId))
			{
				return new Result<UserDto>(new  DbEntityExistsError("User is already in the chat"));
			}
			
			_context.ChatUsers.Add(new ChatUser {UserId = request.UserId, ChatId = request.ChatId});
			await _context.SaveChangesAsync(cancellationToken);
			
			return new Result<UserDto>(new UserDto(user));
		}
		
		return new Result<UserDto>(new ForbiddenError("Not right to add a user to the conversation"));
	}
}