using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class UnbanUserInConversationCommand : IRequest<UserDto>
{
	public Guid RequesterId { get; set; }

	public Guid ChatId { get; set; }
	
	public Guid UserId { get; set; }
}