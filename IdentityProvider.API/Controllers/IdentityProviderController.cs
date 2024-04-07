using IdentityProvider.Application.Service;
using IdentityProvider.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace SmartId.Authentication.Controllers;

[ApiController]
[Route("identity")]
public class IdentityProviderController(ISmartIdAuthenticationService service) : ControllerBase
{
	[HttpPost("authenticate")]
	public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequestModel request)
	{
		var result = await service.RequestAuthentication(request.IdCode, request.CountryCode);
		return Ok(result);
	}
}