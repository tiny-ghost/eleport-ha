namespace SmartId.Authentication.Domain.Settings;

public sealed class SmartIdSettings
{
	public required string RelyingPartyUuid { get; init; }
	public required string RelyingPartyName { get; init; }
	public required string BaseUrlV1 { get; init; }
	public required string BaseUrlV2 { get; init; }
}