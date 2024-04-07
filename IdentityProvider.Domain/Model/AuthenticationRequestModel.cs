namespace IdentityProvider.Domain.Model;

public class AuthenticationRequestModel
{
	public required string IdCode { get; set; }
	public required string CountryCode { get; set; }
}