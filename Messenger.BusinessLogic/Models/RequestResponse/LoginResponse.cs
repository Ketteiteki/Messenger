using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class LoginResponse
{
	public string AccessToken { get; set; } = null!;
	
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; } = null!;
	
	public string NickName { get; set; } = null!;

	public string? AvatarLink { get; set; }
	
	public LoginResponse(User user, string accessToken)
	{
		AccessToken = accessToken;
		Id = user.Id;
		DisplayName = user.DisplayName;
		NickName = user.NickName;
		AvatarLink = user.AvatarLink;
	}
}