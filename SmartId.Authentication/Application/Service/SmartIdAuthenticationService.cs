using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using SmartId.Authentication.Domain.Settings;

namespace SmartId.Authentication.Application.Service;

public class SmartIdAuthenticationService(IHttpClientFactory httpClientFactory, IOptions<SmartIdSettings> options)
{
	public async Task<AuthenticationResponse> RequestAuthentication(string idCode, string countryCode)
	{
		var client = httpClientFactory.CreateClient("SmartIdV2");
		var sessionCredentials = GenerationSessionCredentials();
		var smartIdSettings = options.Value;

		var payload = new PayLoad
		{
			relyingPartyUUID = smartIdSettings.RelyingPartyUuid,
			relyingPartyName = smartIdSettings.RelyingPartyName,
			hash = sessionCredentials.SessionHash,
			hashType = "SHA512",
			allowedInteractionsOrder = [
				new {type = "displayTextAndPIN", displayText60 = "Some text to display..."}
			]
		};

		try
		{
			var response = await client.PostAsJsonAsync($"authentication/etsi/PNO{countryCode}-{idCode}", payload);
			var content = await response.Content.ReadAsStringAsync();

			return new AuthenticationResponse
			{
				Response = content,
				VerificationCode = sessionCredentials.VerificationCode
			};
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}
	} 
	
	public async Task<string> VerifyAuthentication(string sessionId)
	{
		var client = httpClientFactory.CreateClient("SmartIdV2");
		var response = await client.GetAsync($"session/{sessionId}");
		var content = await response.Content.ReadAsStringAsync();

		return content;
	}
	
	public class PayLoad
	{
		public string relyingPartyUUID { get; set; }
		public string relyingPartyName { get; set; }
		public string hash { get; set; }
		public string hashType { get; set; }
		public object[] allowedInteractionsOrder { get; set; }
	}
	public record SessionCredentials(string SessionHash, int VerificationCode);
	public class AuthenticationResponse()
	{
		public string? Response { get; set; }
		public int VerificationCode  { get; set; }
	}
	
	private SessionCredentials GenerationSessionCredentials()
    {
        var randBytes = new byte[64];
        randBytes = RandomNumberGenerator.GetBytes(randBytes.Length);
        var hash = SHA512.HashData(randBytes);
        int code = GenerateVerificationCode(hash);
        var base64String = Convert.ToBase64String(hash);
        return new SessionCredentials(base64String, code);
    }

    private int GenerateVerificationCode(byte[] hash)
	{
		var rightMostBytes = SHA256.HashData(hash)[^2..];
		var code = BitConverter.ToUInt16(rightMostBytes, 0) % 10000; 

		return code;
	}
}