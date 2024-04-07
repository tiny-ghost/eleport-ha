using IdentityProvider.Domain.Model;

namespace IdentityProvider.Application.Service;

public interface ISmartIdAuthenticationService
{
	Task<AuthenticationResponseModel> RequestAuthentication(string idCode, string countryCode);
	Task<string> VerifyAuthentication(string sessionId);
}