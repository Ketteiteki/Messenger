using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatListQueryHandlerTests;

public class GetChatListTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createFirstConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "conv1",
			Title: "conv1",
			Type: ChatType.Conversation,
			AvatarFile: null);
		
		var createSecondConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "conv2",
			Title: "conv2",
			Type: ChatType.Conversation,
			AvatarFile: null);
		
		var createThirdConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "conv3",
			Title: "conv3",
			Type: ChatType.Conversation,
			AvatarFile: null);

		var createDialogCommand = new CreateDialogCommand(
			RequesterId: user21Th.Value.Id,
			UserId: alice.Value.Id);

		var createFirstConversationResult = 
			await MessengerModule.RequestAsync(createFirstConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(createSecondConversationCommand, CancellationToken.None);
		var conversation3 = await MessengerModule.RequestAsync(createThirdConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);

		var createMessageBy21ThCommand = new CreateMessageCommand(
			RequesterId: user21Th.Value.Id,
			Text: "Hello",
			ChatId: createFirstConversationResult.Value.Id,
			ReplyToId: null,
			Files: null);
		
		var createMessageBy21ThResult = await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);
		
		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation3.Value.Id), CancellationToken.None);

		await MessengerModule.RequestAsync(new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation3.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "qwerty",
			RoleColor.Black,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: false), CancellationToken.None);
		
		var getChatListBy21ThResult = await MessengerModule.RequestAsync(new GetChatListQuery(
			RequesterId: user21Th.Value.Id), CancellationToken.None);
		var getChatListByAliceResult = await MessengerModule.RequestAsync(new GetChatListQuery(
			RequesterId: alice.Value.Id), CancellationToken.None);

		foreach (var chat in getChatListBy21ThResult.Value)
		{
			if (chat.Id == createFirstConversationResult.Value.Id)
			{
				chat.LastMessageId.Should().Be(createMessageBy21ThResult.Value.Id);
				chat.LastMessageText.Should().Be(createMessageBy21ThResult.Value.Text);
				chat.LastMessageAuthorDisplayName.Should().Be(createMessageBy21ThResult.Value.OwnerDisplayName);
				chat.LastMessageDateOfCreate.Should().NotBeNull();
			}
			
			if (chat.Type == ChatType.Dialog)
			{
				chat.IsMember.Should().Be(true);
				chat.IsOwner.Should().Be(false);
				chat.Members.Count.Should().Be(2);
				
				continue;
			}
			
			chat.IsMember.Should().Be(true);
			chat.IsOwner.Should().Be(true);
		}
		
		foreach (var chat in getChatListByAliceResult.Value)
		{
			chat.IsMember.Should().Be(true);
			chat.IsOwner.Should().Be(false);
		}
	}
}