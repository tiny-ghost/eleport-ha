using System.Collections;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using IdentityProvider.Domain.Model;
using IdentityProvider.Domain.Settings;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Application.Service;

public class SmartIdAuthenticationService(IHttpClientFactory httpClientFactory, IOptions<SmartIdSettings> options)
	: ISmartIdAuthenticationService
{
	public async Task<AuthenticationResponseModel> RequestAuthentication(string idCode, string countryCode)
	{
		var client = httpClientFactory.CreateClient("SmartIdV2");
		var sessionCredentials = GenerationSessionCredentials();
		var payload = CreateRequestBody(options, sessionCredentials);
		var opts = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};
		
		try
		{
			var response = await client.PostAsJsonAsync($"authentication/etsi/PNO{countryCode}-{idCode}", payload);
			var content = await response.Content.ReadAsStringAsync();
			var data = JsonSerializer.Deserialize<JsonNode>(content);
			if (data!["sessionID"] is not null)
			{
				return new AuthenticationResponseModel
				{
					SuccessResponse = new SmartIdSuccessResponse
					{
						SessionId = data["sessionID"]!.ToString(),
						VerificationCode = sessionCredentials.VerificationCode.ToString()
					}
				};
			}
			
			var errorResponse = new AuthenticationResponseModel
			{
				ErrorResponse = JsonSerializer.Deserialize<SmartIdErrorResponse>(content, opts),
				IsError = true,
			};
			
			return errorResponse;
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
		[JsonPropertyName("relyingPartyUUID")]
		public string? RelyingPartyUuid { get; set; }
		[JsonPropertyName("relyingPartyName")]
		public string? RelyingPartyName { get; set; }
		[JsonPropertyName("hash")]
		public string? Hash { get; set; }
		[JsonPropertyName("hashType")]
		public string? HashType { get; set; }
		[JsonPropertyName("allowedInteractionsOrder")]
		public object[]? AllowedInteractionsOrder { get; set; }
	}

	public record SessionCredentials(string SessionHash, int VerificationCode);


	private static PayLoad CreateRequestBody(IOptions<SmartIdSettings> settings, SessionCredentials sessionCredentials)
	{
		return new PayLoad
		{
			RelyingPartyUuid = settings.Value.RelyingPartyUuid,
			RelyingPartyName = settings.Value.RelyingPartyName,
			Hash = sessionCredentials.SessionHash,
			HashType = "SHA512",
			AllowedInteractionsOrder =
			[
				new {type = "displayTextAndPIN", displayText60 = "Some text to display..."}
			]
		};
		
	}

	private SessionCredentials GenerationSessionCredentials()
	{
		var randBytes = new byte[64];
		randBytes = RandomNumberGenerator.GetBytes(randBytes.Length);
		var hash = SHA512.HashData(randBytes);
		var code = GenerateVerificationCode(hash);
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