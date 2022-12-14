using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.AddUserToConversationCommandHandlerTests;

public class AddUserToConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);
		
        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var addUserAliceToConversationByBobCommand = new AddUserToConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id);

        var addUserAliceToConversationByBobResult =
            await MessengerModule.RequestAsync(addUserAliceToConversationByBobCommand, CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        var addUserBobToConversationByAliceCommand = new AddUserToConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id);

        var addUserBobToConversationByAliceResult =
            await MessengerModule.RequestAsync(addUserBobToConversationByAliceCommand, CancellationToken.None);

        addUserAliceToConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        addUserBobToConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}