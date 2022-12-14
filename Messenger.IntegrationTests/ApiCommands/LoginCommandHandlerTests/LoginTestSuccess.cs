using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.LoginCommandHandlerTests;

public class LoginTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var loginResult = await MessengerModule.RequestAsync(new LoginCommand(
			Nickname: CommandHelper.Registration21ThCommand().Nickname,
			Password: CommandHelper.Registration21ThCommand().Password,
			UserAgent: "Mozilla",
			Ip: "323.432.21.542"), CancellationToken.None);

		loginResult.IsSuccess.Should().BeTrue();
	}
}