using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messanger.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IMediator _mediator;

	public UsersController(IMediator mediator)
	{
		_mediator = mediator;
	}
	
	[Authorize]
	[HttpGet]
	public async Task<IActionResult> GetUsers([FromQuery] string search, [FromQuery] int count = 10, [FromQuery] int page = 1)
	{
		throw new NotImplementedException();
	}
	
	[Authorize]
	[HttpGet("{userId}")]
	public async Task<IActionResult> GetUser(Guid userId)
	{
		throw new NotImplementedException();
	}
}